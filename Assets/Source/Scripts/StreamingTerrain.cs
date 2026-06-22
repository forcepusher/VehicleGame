using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

namespace BananaParty.VehicleGame
{
    public class StreamingTerrain : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float loadDistance = 4000f;
        [SerializeField] private float tileSize = 2000f;
        [SerializeField] private int gridDimension = 10;
        [SerializeField] private float updateInterval = 0.5f;

        private readonly Dictionary<(int, int), GameObject> loadedTiles = new();
        private readonly HashSet<(int, int)> loadingIndices = new();

        [Header("Assets")]
        [SerializeField] private Shader terrainShader;
        [SerializeField] private Texture2D detailAlbedoMap;
        [SerializeField] private float detailTiling = 100f;
        [SerializeField] private float detailStrength = 1f;

        private float gridOffset;

        private void Awake()
        {
            gridOffset = (gridDimension * tileSize) * 0.5f;
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                Debug.LogError("Player Transform not assigned to StreamingTerrain");
                return;
            }
            StartCoroutine(StreamingRoutine());
        }

        private IEnumerator StreamingRoutine()
        {
            while (true)
            {
                UpdateStreaming();
                yield return new WaitForSeconds(updateInterval);
            }
        }

        private void UpdateStreaming()
        {
            Vector3 playerPos = playerTransform.position;
            HashSet<(int, int)> requiredIndices = new();

            int centerTileX = Mathf.RoundToInt((playerPos.x + gridOffset) / tileSize);
            int centerTileY = Mathf.RoundToInt((playerPos.z + gridOffset) / tileSize);

            int radius = Mathf.CeilToInt(loadDistance / tileSize);

            for (int x = centerTileX - radius; x <= centerTileX + radius; x++)
            {
                if (x < 0 || x >= gridDimension) continue;
                for (int y = centerTileY - radius; y <= centerTileY + radius; y++)
                {
                    if (y < 0 || y >= gridDimension) continue;

                    float minX = x * tileSize - gridOffset;
                    float maxX = minX + tileSize;
                    float minY = y * tileSize - gridOffset;
                    float maxY = minY + tileSize;

                    if (IsPlayerNearTile(playerPos, minX, maxX, minY, maxY))
                    {
                        requiredIndices.Add((x, y));
                    }
                }
            }

            // Unload tiles no longer needed
            List<(int, int)> toRemove = new();
            foreach (var index in loadedTiles.Keys)
            {
                if (!requiredIndices.Contains(index))
                {
                    toRemove.Add(index);
                }
            }

            foreach (var index in toRemove)
            {
                UnloadTile(index);
            }

            // Load new tiles async
            foreach (var index in requiredIndices)
            {
                if (!loadedTiles.ContainsKey(index) && !loadingIndices.Contains(index))
                {
                    StartCoroutine(LoadTileAsync(index));
                }
            }
        }

        private bool IsPlayerNearTile(Vector3 pos, float minX, float maxX, float minY, float maxY)
        {
            float dx = Mathf.Max(0, Mathf.Max(minX - pos.x, pos.x - maxX));
            float dz = Mathf.Max(0, Mathf.Max(minY - pos.z, pos.z - maxY));
            return (dx * dx + dz * dz) <= loadDistance * loadDistance;
        }

        private IEnumerator LoadTileAsync((int x, int y) index)
        {
            loadingIndices.Add(index);

            string bundleName = $"Terrain_x0{index.x}_y0{index.y}";
            string path = Path.Combine(Application.streamingAssetsPath, bundleName);

            AssetBundle bundle = null;

#if UNITY_WEBGL && !UNITY_EDITOR
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load AssetBundle at {path}: {request.error}");
                    loadingIndices.Remove(index);
                    yield break;
                }
                bundle = DownloadHandlerAssetBundle.GetContent(request);
            }
#else
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;
            bundle = bundleRequest.assetBundle;
#endif

            if (bundle == null)
            {
                Debug.LogError($"Failed to load AssetBundle at {path}");
                loadingIndices.Remove(index);
                yield break;
            }

            string[] assetNames = bundle.GetAllAssetNames();
            if (assetNames == null || assetNames.Length == 0)
            {
                Debug.LogError($"AssetBundle {bundleName} contains no assets.");
                bundle.Unload(true);
                loadingIndices.Remove(index);
                yield break;
            }

            Mesh mesh = null;
            Texture2D texture = null;

            foreach (var assetName in assetNames)
            {
                string lowerName = assetName.ToLower();
                if (lowerName.Contains("mesh output"))
                {
                    AssetBundleRequest request = bundle.LoadAssetAsync<Mesh>(assetName);
                    yield return request;
                    mesh = request.asset as Mesh;
                }
                else if (lowerName.Contains("bitmap output"))
                {
                    AssetBundleRequest request = bundle.LoadAssetAsync<Texture2D>(assetName);
                    yield return request;
                    texture = request.asset as Texture2D;
                }
            }

            if (mesh == null)
            {
                Debug.LogError($"Bundle {bundleName} is missing a mesh asset containing 'Mesh Output'.");
                bundle.Unload(true);
                loadingIndices.Remove(index);
                yield break;
            }

            GameObject tileRoot = new GameObject($"Tile_{index.x}_{index.y}");
            tileRoot.transform.position = Vector3.zero;

            GameObject meshObj = new GameObject($"Mesh_{index.x}_{index.y}");
            meshObj.transform.SetParent(tileRoot.transform);

            MeshFilter filter = meshObj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = meshObj.AddComponent<MeshRenderer>();
            Shader shaderToUse = terrainShader != null ? terrainShader : Shader.Find("Custom/BakedTerrain");
            Material mat = new Material(shaderToUse);

            if (texture != null)
            {
                mat.SetTexture("_MainTex", texture);
            }

            if (detailAlbedoMap != null)
            {
                mat.SetTexture("_DetailAlbedoMap", detailAlbedoMap);
                mat.SetTextureScale("_DetailAlbedoMap", new Vector2(detailTiling, detailTiling));
            }

            mat.SetFloat("_DetailStrength", detailStrength);
            renderer.material = mat;

            MeshCollider collider = meshObj.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            loadedTiles.Add(index, tileRoot);
            bundle.Unload(false);
            loadingIndices.Remove(index);
        }

        private void UnloadTile((int x, int y) index)
        {
            if (loadedTiles.TryGetValue(index, out GameObject tileObj))
            {
                Destroy(tileObj);
                loadedTiles.Remove(index);
            }
        }
    }
}
