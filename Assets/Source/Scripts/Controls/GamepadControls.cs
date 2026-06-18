using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class GamepadControls : IControls
    {
        public float Roll => Mathf.Clamp(Gamepad.current?.rightStick.ReadValue().x ?? 0f, -1f, 1f); // Left stick X: Roll left-right

        // Left stick Y inverted: push forward (up) = more throttle, pull back = less/reverse
        public float Throttle => Mathf.Clamp((Gamepad.current?.rightTrigger.ReadValue() ?? 0f) - (Gamepad.current?.leftTrigger.ReadValue() ?? 0), -1f, 1f);

        public float Yaw => Mathf.Clamp(Gamepad.current?.leftStick.ReadValue().x ?? 0f, -1f, 1f);

        // Right stick Y inverted: pull back (up) = climb/pitch up, push forward = dive/pitch down
        public float Pitch => Mathf.Clamp(Gamepad.current?.rightStick.ReadValue().y ?? 0f, -1f, 1f);

        public void Update()
        {

        }
    }
}
