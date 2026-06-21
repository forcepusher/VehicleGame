using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class FrameRateDisplay : MonoBehaviour
    {
        private float _deltaTime = 0.0f;
        private Rect _fpsRect = new Rect(10, 10, 200, 30);
        private GUIStyle _style = new GUIStyle();

        private void Start()
        {
            _style.fontSize = 20;
            _style.normal.textColor = Color.white;
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            float fps = 1.0f / _deltaTime;
            float width = 200;
            float height = 30;
            Rect rect = new Rect(Screen.width - width - 10, Screen.height - height - 10, width, height);

            GUI.Label(rect, "FPS: " + Mathf.Round(fps), _style);
        }
    }
}
