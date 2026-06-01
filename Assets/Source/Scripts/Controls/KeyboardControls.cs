using UnityEngine.InputSystem;

namespace Igrushka.VehicleGame
{
    public class KeyboardControls : IControls
    {
        public float Throttle =>
            (Keyboard.current?.wKey.isPressed == true ? 1f : 0f) +
            (Keyboard.current?.sKey.isPressed == true ? -1f : 0f);

        public float Turn =>
            (Keyboard.current?.dKey.isPressed == true ? 1f : 0f) +
            (Keyboard.current?.aKey.isPressed == true ? -1f : 0f);

        public float Yaw => Mouse.current?.delta.ReadValue().x ?? 0f;

        public float Pitch => Mouse.current?.delta.ReadValue().y ?? 0f;

        public void Update()
        {

        }
    }
}
