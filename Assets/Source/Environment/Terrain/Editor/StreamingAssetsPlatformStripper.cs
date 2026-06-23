using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BananaParty.VehicleGame.Editor
{
    public class StreamingAssetsPlatformStripper : IPostprocessBuildWithReport
    {
        private static readonly string[] PlatformFolders = { "WebGL", "Standalone" };

        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            string platformFolderToKeep = GetPlatformFolderToKeep(report.summary.platform);
            if (platformFolderToKeep == null)
                return;

            string streamingAssetsPath = GetBuiltStreamingAssetsPath(report);
            if (streamingAssetsPath == null || !Directory.Exists(streamingAssetsPath))
                return;

            foreach (string platformFolder in PlatformFolders)
            {
                if (platformFolder == platformFolderToKeep)
                    continue;

                string pathToRemove = Path.Combine(streamingAssetsPath, platformFolder);
                if (!Directory.Exists(pathToRemove))
                    continue;

                Directory.Delete(pathToRemove, true);
                Debug.Log($"Removed unused StreamingAssets platform folder from build: {pathToRemove}");
            }
        }

        private static string GetPlatformFolderToKeep(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    return "Standalone";
                default:
                    return null;
            }
        }

        private static string GetBuiltStreamingAssetsPath(BuildReport report)
        {
            string outputPath = report.summary.outputPath;

            switch (report.summary.platform)
            {
                case BuildTarget.WebGL:
                {
                    string buildFolder = Directory.Exists(outputPath) ? outputPath : Path.GetDirectoryName(outputPath);
                    return Path.Combine(buildFolder, "StreamingAssets");
                }
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                {
                    string buildFolder = Path.GetDirectoryName(outputPath);
                    string dataFolder = Path.Combine(buildFolder, Path.GetFileNameWithoutExtension(outputPath) + "_Data");
                    return Path.Combine(dataFolder, "StreamingAssets");
                }
                default:
                    return null;
            }
        }
    }
}
