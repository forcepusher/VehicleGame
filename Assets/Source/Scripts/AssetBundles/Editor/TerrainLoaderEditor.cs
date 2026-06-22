using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace BananaParty.VehicleGame
{
    public class TerrainLoaderEditor : EditorWindow
    {
        [MenuItem("AssetBundles/Load All Terrain Assets")]
        public static void LoadAllTerrain()
        {
            // Configuration matching StreamingTerrain
            int gridDimension = 10;
            float tileSize = 2000f;
            float gridOffset = (gridDimension * tileSize) * 0.5f;

            // We need a reference to the shader and detail map.
            // Since we are in an editor script, we'll try to find them or ask user to set them.
            // For simplicity, we can search for the assets by name or use defaults.
            Shader terrainShader = Shader.Find("Custom/BakedTerrain");

            GameObject root = new GameObject("LoadedTerrain_All");

            for (int x = 0; x < gridDimension; x++)
            {
                for (int y = 0; y < gridDimension; y++)
                {
                    // Apply the same inversion as StreamingTerrain.cs
                    int assetX = (gridDimension - 1) - x;
                    string bundleName = $"Terrain_x0{assetX}_y0{y}";
                    string path = Path.Combine(Application.streamingAssetsPath, bundleName);

                    AssetBundle bundle = AssetBundle.LoadFromFile(path);
                    if (bundle == null)
                    {
                        Debug.LogError($"Failed to load AssetBundle at {path}");
                        continue;
                    }

                    string[] assetNames = bundle.GetAllAssetNames();
                    Mesh mesh = null;
                    Texture2D texture = null;

                    foreach (var assetName in assetNames)
                    {
                        string lowerName = assetName.ToLower();
                        if (lowerName.Contains("mesh output"))
                        {
                            mesh = bundle.LoadAsset<Mesh>(assetName);
                        }
                        else if (lowerName.Contains("bitmap output"))
                        {
                            texture = bundle.LoadAsset<Texture2D>(assetName);
                        }
                    }

                    if (mesh != null)
                    {
                        GameObject tileRoot = new GameObject($"Tile_{x}_{y}");
                        tileRoot.transform.SetParent(root.transform);
                        tileRoot.transform.position = Vector3.zero;

                        GameObject meshObj = new GameObject($"Mesh_{x}_{y}");
                        meshObj.transform.SetParent(tileRoot.transform);

                        MeshFilter filter = meshObj.AddComponent<MeshFilter>();
                        filter.sharedMesh = mesh;

                        MeshRenderer renderer = meshObj.AddComponent<MeshRenderer>();
                        Material mat = new Material(terrainShader != null ? terrainShader : Shader.Find("Custom/BakedTerrain"));

                        if (texture != null)
                        {
                            mat.SetTexture("_MainTex", texture);
                        }

                        renderer.material = mat;
                        meshObj.AddComponent<MeshCollider>().sharedMesh = mesh;
                    }

                    bundle.Unload(false);
                }
            }

            Debug.Log("Successfully loaded all terrain assets.");
        }
    }
}
