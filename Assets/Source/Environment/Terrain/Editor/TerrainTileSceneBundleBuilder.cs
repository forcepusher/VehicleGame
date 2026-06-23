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
        private const string WebGlFolder = "Assets/StreamingAssets/WebGL";
        private const string SceneBundlesFolder = "Assets/StreamingAssets/WebGL/SceneBundles";

        [MenuItem("Tools/Terrain/Build Tile Scene Bundles/WebGL")]
        public static void BuildTileSceneBundlesForWebGl()
        {
            BuildTileSceneBundlesForTarget(BuildTarget.WebGL);
        }

        private static void BuildTileSceneBundlesForTarget(BuildTarget buildTarget)
        {
            string[] scenePaths = CollectTileScenePaths();
            string outputDirectory = SceneBundlesFolder;
            AssetBundleBuild[] bundleDefinitions = CreateBundleDefinitions(scenePaths);

            EnsureOutputDirectoryExists();
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
                $"Load at runtime via Application.streamingAssetsPath. " +
                $"Manifest bundles: {string.Join(", ", manifest.GetAllAssetBundles())}.");
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

        private static void EnsureOutputDirectoryExists()
        {
            if (!AssetDatabase.IsValidFolder(StreamingAssetsFolder))
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");

            if (!AssetDatabase.IsValidFolder(WebGlFolder))
                AssetDatabase.CreateFolder(StreamingAssetsFolder, "WebGL");

            if (!AssetDatabase.IsValidFolder(SceneBundlesFolder))
                AssetDatabase.CreateFolder(WebGlFolder, "SceneBundles");
        }
    }
}
