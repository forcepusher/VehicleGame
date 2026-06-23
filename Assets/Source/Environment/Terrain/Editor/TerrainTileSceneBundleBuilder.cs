using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BananaParty.VehicleGame.Editor
{
    public static class TerrainTileSceneBundleBuilder
    {
        private const string ScenesFolder = "Assets/Source/Scenes/Tiles";
        private const string StreamingAssetsFolder = "Assets/StreamingAssets";
        private const string SceneTilesFolderName = "SceneTiles";
        private const string WebGlPlatformFolderName = "WebGL";
        private const string StandalonePlatformFolderName = "Standalone";

        [MenuItem("Tools/Terrain/Build Tile Scene Bundles/WebGL")]
        public static void BuildTileSceneBundlesForWebGl()
        {
            BuildTileSceneBundlesForTarget(BuildTarget.WebGL, WebGlPlatformFolderName);
        }

        [MenuItem("Tools/Terrain/Build Tile Scene Bundles/Standalone")]
        public static void BuildTileSceneBundlesForStandalone()
        {
            BuildTileSceneBundlesForTarget(BuildTarget.StandaloneWindows64, StandalonePlatformFolderName);
        }

        private static void BuildTileSceneBundlesForTarget(BuildTarget buildTarget, string platformFolderName)
        {
            string[] scenePaths = CollectTileScenePaths();
            string outputDirectory = GetOutputDirectory(platformFolderName);
            AssetBundleBuild[] bundleDefinitions = CreateBundleDefinitions(scenePaths);

            EnsurePlatformFolderExists(platformFolderName);
            Directory.CreateDirectory(outputDirectory);

            BuildAssetBundlesParameters buildParameters = new BuildAssetBundlesParameters
            {
                outputPath = outputDirectory,
                bundleDefinitions = bundleDefinitions,
                targetPlatform = buildTarget,
                options = BuildAssetBundleOptions.ChunkBasedCompression,
            };

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildParameters);
            AssetDatabase.Refresh();

            Debug.Log(
                $"Built {bundleDefinitions.Length} tile scene bundles for {buildTarget} at {outputDirectory}. " +
                $"Load at runtime via Application.streamingAssetsPath/{platformFolderName}/{SceneTilesFolderName}. " +
                $"Manifest bundles: {string.Join(", ", manifest.GetAllAssetBundles())}.");
        }

        private static string GetOutputDirectory(string platformFolderName)
        {
            return $"{StreamingAssetsFolder}/{platformFolderName}/{SceneTilesFolderName}";
        }

        private static AssetBundleBuild[] CreateBundleDefinitions(string[] scenePaths)
        {
            AssetBundleBuild[] bundleDefinitions = new AssetBundleBuild[scenePaths.Length];

            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                string tileId = Path.GetFileNameWithoutExtension(scenePath);

                bundleDefinitions[i] = new AssetBundleBuild
                {
                    assetBundleName = tileId,
                    assetNames = new[] { scenePath },
                };
            }

            return bundleDefinitions;
        }

        private static string[] CollectTileScenePaths()
        {
            List<string> scenePaths = AssetDatabase
                .FindAssets("t:Scene", new[] { ScenesFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".unity"))
                .OrderBy(path => path)
                .ToList();

            if (scenePaths.Count == 0)
                throw new BuildFailedException($"No tile scenes found in {ScenesFolder}. Run Generate Tile Scenes first.");

            return scenePaths.ToArray();
        }

        private static void EnsurePlatformFolderExists(string platformFolderName)
        {
            if (!AssetDatabase.IsValidFolder(StreamingAssetsFolder))
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");

            string platformFolder = $"{StreamingAssetsFolder}/{platformFolderName}";
            if (!AssetDatabase.IsValidFolder(platformFolder))
                AssetDatabase.CreateFolder(StreamingAssetsFolder, platformFolderName);

            string sceneBundlesFolder = GetOutputDirectory(platformFolderName);
            if (!AssetDatabase.IsValidFolder(sceneBundlesFolder))
                AssetDatabase.CreateFolder(platformFolder, SceneTilesFolderName);
        }
    }
}
