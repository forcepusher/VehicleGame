using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BananaParty.VehicleGame.Editor
{
    public static class TerrainTileMaterialApplier
    {
        private const string TilesFolder = "Assets/Source/Environment/Terrain/Tiles";
        private const string MaterialsFolder = "Assets/Source/Environment/Terrain/Tiles/Materials";

        [MenuItem("Tools/Terrain/Apply Tile Materials In Scene")]
        public static void ApplyTileMaterialsInScene()
        {
            Dictionary<string, Material> materialsByName = LoadMaterialsByName();
            MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            int appliedCount = 0;

            foreach (MeshRenderer renderer in renderers)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                Mesh mesh = meshFilter.sharedMesh;
                string meshPath = AssetDatabase.GetAssetPath(mesh);

                if (!meshPath.StartsWith(TilesFolder))
                    continue;

                string meshName = Path.GetFileNameWithoutExtension(meshPath);
                Material material = materialsByName[meshName];

                Undo.RecordObject(renderer, "Apply Tile Material");
                renderer.sharedMaterial = material;
                appliedCount++;
            }

            Debug.Log($"Applied tile materials to {appliedCount} renderers.");
        }

        private static Dictionary<string, Material> LoadMaterialsByName()
        {
            Dictionary<string, Material> materialsByName = new Dictionary<string, Material>();

            foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { MaterialsFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                materialsByName[material.name] = material;
            }

            return materialsByName;
        }
    }
}
