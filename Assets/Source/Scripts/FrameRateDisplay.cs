using UnityEngine;

namespace Igrushka.VehicleGame
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

            GUI.Label(_fpsRect, "FPS: " + Mathf.Round(fps), _style);
        }
    }
}
