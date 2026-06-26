using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class KeyboardControls : IControls
    {
        private const float MouseSensitivity = 0.1f;

        public float Throttle => Mathf.Clamp(
            (Keyboard.current?.wKey.isPressed == true ? 1f : 0f) +
            (Keyboard.current?.sKey.isPressed == true ? -1f : 0f), -1f, 1f);

        public float Roll => Mathf.Clamp(
            (Keyboard.current?.dKey.isPressed == true ? 1f : 0f) +
            (Keyboard.current?.aKey.isPressed == true ? -1f : 0f), -1f, 1f);

        public float Yaw => Mathf.Clamp(
            (Mouse.current?.delta.ReadValue().x ?? 0f) * MouseSensitivity, -1f, 1f);

        public float Pitch => Mathf.Clamp(
            ((Mouse.current?.delta.ReadValue().y ?? 0f) * MouseSensitivity) + (Keyboard.current?.spaceKey.isPressed == true ? 1f : 0f), -1f, 1f);

        public bool FirePrimary => Mouse.current?.leftButton.ReadValue() != 0 ? true : false;

        public bool FireSecondary => Mouse.current?.rightButton.ReadValue() != 0 ? true : false;

        public void ManualUpdate()
        {

        }
    }
}
