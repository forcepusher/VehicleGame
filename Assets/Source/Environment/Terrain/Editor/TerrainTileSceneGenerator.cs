using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BananaParty.VehicleGame.Editor
{
    public static class TerrainTileSceneGenerator
    {
        private const string TilesFolder = "Assets/Source/Environment/Terrain/Tiles";
        private const string MaterialsFolder = "Assets/Source/Environment/Terrain/Tiles/Materials";
        private const string ScenesFolder = "Assets/Source/Scenes/Tiles";
        private const string MeshPrefix = "TerrainMesh_";

        [MenuItem("Tools/Terrain/Generate Tile Scenes")]
        public static void GenerateTileScenes()
        {
            EnsureScenesFolderExists();

            Dictionary<string, Material> materialsByName = LoadMaterialsByName();
            SceneSetup[] previousSetup = EditorSceneManager.GetSceneManagerSetup();
            List<string> meshPaths = CollectTileMeshPaths();
            int generatedCount = 0;

            try
            {
                for (int i = 0; i < meshPaths.Count; i++)
                {
                    string meshPath = meshPaths[i];
                    string meshName = Path.GetFileNameWithoutExtension(meshPath);
                    string tileId = meshName.Substring(MeshPrefix.Length);
                    string scenePath = $"{ScenesFolder}/{tileId}.unity";

                    EditorUtility.DisplayProgressBar(
                        "Generating Tile Scenes",
                        tileId,
                        (float)i / meshPaths.Count);

                    GenerateTileScene(meshPath, meshName, tileId, scenePath, materialsByName[meshName]);
                    generatedCount++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Generated {generatedCount} tile scenes in {ScenesFolder}.");
        }

        private static void GenerateTileScene(
            string meshPath,
            string meshName,
            string tileId,
            string scenePath,
            Material material)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Mesh mesh = LoadMesh(meshPath);
            GameObject terrainObject = new GameObject(meshName);

            MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshRenderer = terrainObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            MeshCollider meshCollider = terrainObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            GameObjectUtility.SetStaticEditorFlags(
                terrainObject,
                StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic);

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static List<string> CollectTileMeshPaths()
        {
            List<string> meshPaths = new List<string>();

            foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { TilesFolder }))
            {
                string meshPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!meshPath.EndsWith(".obj"))
                    continue;

                string meshName = Path.GetFileNameWithoutExtension(meshPath);
                if (!meshName.StartsWith(MeshPrefix))
                    continue;

                meshPaths.Add(meshPath);
            }

            meshPaths.Sort();
            return meshPaths;
        }

        private static Mesh LoadMesh(string meshPath)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(meshPath);

            foreach (Object asset in assets)
            {
                Mesh mesh = asset as Mesh;
                if (mesh != null)
                    return mesh;
            }

            throw new FileNotFoundException($"No mesh found in {meshPath}.");
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

        private static void EnsureScenesFolderExists()
        {
            if (!AssetDatabase.IsValidFolder(ScenesFolder))
                AssetDatabase.CreateFolder("Assets/Source/Scenes", "Tiles");
        }
    }
}
