using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace BananaParty.VehicleGame
{
    public class StreamingTerrain : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float loadDistance = 4000f; // Load tiles within this distance
        [SerializeField] private float tileSize = 2000f;
        [SerializeField] private int gridDimension = 10;

        private readonly Dictionary<(int, int), GameObject> loadedTiles = new();
        private readonly HashSet<(int, int)> activeIndices = new();

        private void Update()
        {
            if (playerTransform == null) return;

            UpdateStreaming();
        }

        private void UpdateStreaming()
        {
            Vector3 playerPos = playerTransform.position;
            HashSet<(int, int)> requiredIndices = new();

            // Calculate which tiles should be loaded based on player position
            // Assuming the grid spans from -10km to 10km (10 tiles of 2km)
            // Tile index X=0 is at -10k to -8k, etc.
            int centerTileX = Mathf.RoundToInt((playerPos.x + 10000f) / tileSize);
            int centerTileY = Mathf.RoundToInt((playerPos.z + 10000f) / tileSize);

            int radius = Mathf.CeilToInt(loadDistance / tileSize);

            for (int x = centerTileX - radius; x <= centerTileX + radius; x++)
            {
                for (int y = centerTileY - radius; y <= centerTileY + radius; y++)
                {
                    if (x >= 0 && x < gridDimension && y >= 0 && y < gridDimension)
                    {
                        // Check if player is actually within distance of this tile's bounding box
                        float minX = x * tileSize - 10000f;
                        float maxX = minX + tileSize;
                        float minY = y * tileSize - 10000f;
                        float maxY = minY + tileSize;

                        if (IsPlayerNearTile(playerPos, minX, maxX, minY, maxY))
                        {
                            requiredIndices.Add((x, y));
                        }
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

            // Load new tiles
            foreach (var index in requiredIndices)
            {
                if (!loadedTiles.ContainsKey(index))
                {
                    LoadTile(index);
                }
            }
        }

        private bool IsPlayerNearTile(Vector3 pos, float minX, float maxX, float minY, float maxY)
        {
            float dx = Mathf.Max(0, Mathf.Max(minX - pos.x, pos.x - maxX));
            float dz = Mathf.Max(0, Mathf.Max(minY - pos.z, pos.z - maxY));
            return (dx * dx + dz * dz) <= loadDistance * loadDistance;
        }

        private void LoadTile((int x, int y) index)
        {
            string bundleName = $"Terrain_x0{index.x}_y0{index.y}";
            string path = Path.Combine(Application.streamingAssetsPath, bundleName);

            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                throw new FileNotFoundException($"Failed to load AssetBundle at {path}. Ensure the build pipeline has run and created the file.");
            }

            GameObject tileRoot = new GameObject($"Tile_{index.x}_{index.y}");
            tileRoot.transform.position = Vector3.zero;

            string[] assetNames = bundle.GetAllAssetNames();
            if (assetNames == null || assetNames.Length == 0)
            {
                Debug.LogError($"AssetBundle {bundleName} contains no assets.");
                Destroy(tileRoot);
                bundle.Unload(true);
                return;
            }

            Mesh mesh = null;
            Texture2D texture = null;

            foreach (var assetName in assetNames)
            {
                if (assetName.ToLower().Contains("mesh output"))
                {
                    mesh = bundle.LoadAsset<Mesh>(assetName);
                    if (mesh == null) Debug.LogError($"Asset {assetName} was found but could not be loaded as Mesh in {bundleName}");
                }
                else if (assetName.ToLower().Contains("bitmap output"))
                {
                    texture = bundle.LoadAsset<Texture2D>(assetName);
                    if (texture == null) Debug.LogError($"Asset {assetName} was found but could not be loaded as Texture2D in {bundleName}");
                }
            }

            if (mesh == null)
            {
                Debug.LogError($"Bundle {bundleName} is missing a mesh asset containing 'Mesh Output'. Available assets: {string.Join(", ", assetNames)}");
            }
            else
            {
                GameObject meshObj = new GameObject($"Mesh_{index.x}_{index.y}");
                meshObj.transform.SetParent(tileRoot.transform);

                MeshFilter filter = meshObj.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;

                MeshRenderer renderer = meshObj.AddComponent<MeshRenderer>();
                if (texture != null)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.mainTexture = texture;
                    renderer.material = mat;
                }
                else
                {
                    Debug.LogWarning($"Bundle {bundleName} has a mesh but is missing a 'Bitmap Output' texture.");
                }
            }

            loadedTiles.Add(index, tileRoot);
            bundle.Unload(false);
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
