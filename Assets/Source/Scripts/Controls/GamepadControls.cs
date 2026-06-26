using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class GamepadControls : IControls
    {
        public float Roll => Mathf.Clamp(Gamepad.current?.leftStick.ReadValue().x ?? 0f, -1f, 1f);

        public float Throttle => Mathf.Clamp((Gamepad.current?.rightTrigger.ReadValue() ?? 0f) - (Gamepad.current?.leftTrigger.ReadValue() ?? 0), -1f, 1f);

        public float Yaw => Mathf.Clamp(Gamepad.current?.rightStick.ReadValue().x ?? 0f, -1f, 1f);

        public float Pitch => Mathf.Clamp((Gamepad.current?.rightStick.ReadValue().y ?? 0f) + (Gamepad.current?.leftStick.ReadValue().y ?? 0f), -1f, 1f);

        public void ManualUpdate()
        {

        }
    }
}
