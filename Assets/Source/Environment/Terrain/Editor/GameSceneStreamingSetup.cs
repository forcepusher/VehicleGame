using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BananaParty.VehicleGame.Editor
{
    public static class GameSceneStreamingSetup
    {
        private const string GameScenePath = "Assets/Source/Scenes/GameScene.unity";

        [MenuItem("Tools/Terrain/Setup Game Scene Streaming")]
        public static void SetupGameSceneStreaming()
        {
            Scene gameScene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            VehicleSwitch vehicleSwitch = Object.FindFirstObjectByType<VehicleSwitch>();
            StreamingTerrain streamer = vehicleSwitch.GetComponent<StreamingTerrain>();

            if (streamer == null)
                streamer = vehicleSwitch.gameObject.AddComponent<StreamingTerrain>();

            Transform streamSource = FindStreamSource(vehicleSwitch);
            SerializedObject streamerObject = new SerializedObject(streamer);
            streamerObject.FindProperty("_streamSource").objectReferenceValue = streamSource;
            streamerObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject vehicleSwitchObject = new SerializedObject(vehicleSwitch);
            vehicleSwitchObject.FindProperty("_streamingTerrain").objectReferenceValue = streamer;
            vehicleSwitchObject.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(gameScene);
            EditorSceneManager.SaveScene(gameScene);
            AddGameSceneToBuildSettings();

            Debug.Log("GameScene streaming setup complete.");
        }

        private static Transform FindStreamSource(VehicleSwitch vehicleSwitch)
        {
            SerializedObject vehicleSwitchObject = new SerializedObject(vehicleSwitch);
            SerializedProperty vehiclesProperty = vehicleSwitchObject.FindProperty("_vehicleGameObjects");
            GameObject vehicleObject = vehiclesProperty.GetArrayElementAtIndex(0).objectReferenceValue as GameObject;
            return vehicleObject.transform;
        }

        private static void AddGameSceneToBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(GameScenePath, true),
            };
        }
    }
}
