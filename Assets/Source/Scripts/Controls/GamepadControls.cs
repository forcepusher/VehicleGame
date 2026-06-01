using UnityEngine.InputSystem;

namespace Igrushka.VehicleGame
{
    public class GamepadControls : IControls
    {
        public float Turn => Gamepad.current?.leftStick.ReadValue().x ?? 0f;

        public float Throttle => Gamepad.current?.leftStick.ReadValue().y ?? 0f;

        public float Yaw => Gamepad.current?.rightStick.ReadValue().x ?? 0f;

        public float Pitch => Gamepad.current?.rightStick.ReadValue().y ?? 0f;

        public void Update()
        {

        }
    }
}
