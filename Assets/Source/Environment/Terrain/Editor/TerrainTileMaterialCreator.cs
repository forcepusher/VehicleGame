using System.IO;
using UnityEditor;
using UnityEngine;

namespace BananaParty.VehicleGame.Editor
{
    public static class TerrainTileMaterialCreator
    {
        private const string TilesFolder = "Assets/Source/Environment/Terrain/Tiles";
        private const string MaterialsFolder = "Assets/Source/Environment/Terrain/Tiles/Materials";
        private const string ShaderName = "Custom/BakedTerrain";

        [MenuItem("Tools/Terrain/Create Tile Materials")]
        public static void CreateTileMaterials()
        {
            EnsureMaterialsFolderExists();

            Shader shader = Shader.Find(ShaderName);
            int createdCount = 0;
            int updatedCount = 0;

            foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { TilesFolder }))
            {
                string meshPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!meshPath.EndsWith(".obj"))
                    continue;

                string meshName = Path.GetFileNameWithoutExtension(meshPath);
                string materialPath = $"{MaterialsFolder}/{meshName}.mat";

                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null)
                {
                    material = new Material(shader) { name = meshName };
                    AssetDatabase.CreateAsset(material, materialPath);
                    createdCount++;
                }
                else
                {
                    material.shader = shader;
                    updatedCount++;
                }

                int tileIndex = meshName.IndexOf("_x");
                string tileSuffix = meshName.Substring(tileIndex);
                string texturePath = $"{TilesFolder}/TerrainTexture{tileSuffix}.png";
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                material.SetTexture("_MainTex", texture);
                EditorUtility.SetDirty(material);
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Tile materials: {createdCount} created, {updatedCount} updated in {MaterialsFolder}.");
        }

        private static void EnsureMaterialsFolderExists()
        {
            if (AssetDatabase.IsValidFolder(MaterialsFolder))
                return;

            AssetDatabase.CreateFolder(TilesFolder, "Materials");
        }
    }
}
