using UnityEngine;
using UnityEngine.InputSystem;

namespace Igrushka.VehicleGame
{
    public class KeyboardControls : IControls
    {
        private const float MouseSensitivity = 0.1f;

        public float Throttle => Mathf.Clamp(
            (Keyboard.current?.wKey.isPressed == true ? 1f : 0f) +
            (Keyboard.current?.sKey.isPressed == true ? -1f : 0f), -1f, 1f);

        public float Turn => Mathf.Clamp(
            (Keyboard.current?.dKey.isPressed == true ? 1f : 0f) +
            (Keyboard.current?.aKey.isPressed == true ? -1f : 0f), -1f, 1f);

        public float Yaw => Mathf.Clamp(
            (Mouse.current?.delta.ReadValue().x ?? 0f) * MouseSensitivity, -1f, 1f);

        public float Pitch => Mathf.Clamp(
            (Mouse.current?.delta.ReadValue().y ?? 0f) * MouseSensitivity, -1f, 1f);

        public void Update()
        {

        }
    }
}
