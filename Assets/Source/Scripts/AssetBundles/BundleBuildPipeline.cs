using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public static class BundleBuildPipeline
    {
        [MenuItem("AssetBundles/Build")]
        public static void Build()
        {
            List<AssetBundleBuild> assetBundleDefinitionList = new();

            for (int terrainIndexX = 0; terrainIndexX < 10; terrainIndexX += 1)
            {
                for (int terrainIndexY = 0; terrainIndexY < 10; terrainIndexY += 1)
                {
                    string assetBundleName = $"Terrain_{terrainIndexX}_{terrainIndexY}";

                    AssetBundleBuild assetBundleBuild = new();
                    assetBundleBuild.assetBundleName = $"Terrain_x0{terrainIndexX}_y0{terrainIndexY}";
                    assetBundleBuild.assetNames = new string[]
                    {
                        //$"TerrainMaterial_x0{terrainIndexX}_y0{terrainIndexY}",
                        $"TerrainProject Bitmap Output 4096_x0{terrainIndexX}_y0{terrainIndexY}",
                        $"TerrainProject Mesh Output_x0{terrainIndexX}_y0{terrainIndexY}"
                    };

                    assetBundleDefinitionList.Add(assetBundleBuild);
                }
            }

            BuildAssetBundlesParameters buildParameters = new()
            {
                outputPath = "Assets/StreamingAssets",
                options = BuildAssetBundleOptions.AssetBundleStripUnityVersion,
                bundleDefinitions = assetBundleDefinitionList.ToArray()
            };
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildParameters);
        }
    }
}
