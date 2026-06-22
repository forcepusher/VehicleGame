using UnityEditor;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public static class BundleBuildPipeline
    {
        [MenuItem("AssetBundles/Build")]
        public static void Build()
        {
            for (int terrainIndexX = 0; terrainIndexX < 10; terrainIndexX += 1)
            {
                for (int terrainIndexY = 0; terrainIndexY < 10; terrainIndexY += 1)
                {
                    string assetBundleName = $"Terrain_{terrainIndexX}_{terrainIndexY}";
                    AssetBundleBuild assetBundleBuild = new();
                    assetBundleBuild.assetBundleName = $"Terrain_{terrainIndexX}_{terrainIndexY}";
                }
            }
        }
    }
}
