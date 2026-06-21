using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace BananaParty.VehicleGame
{
    public class VehicleSwitch : MonoBehaviour
    {
        [SerializeField]
        List<IVehicle> _vehicles;

        private CompositeControls _activeControls;
        private NullControls _inactiveControls = new NullControls();
        private int _currentVehicleIndex = 0;

        private void Start()
        {
            if (_vehicles == null || _vehicles.Count == 0) return;

            _activeControls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

            for (int i = 0; i < _vehicles.Count; i++)
            {
                _vehicles[i].SetControls(i == _currentVehicleIndex ? _activeControls : _inactiveControls);
            }
        }

        private void Update()
        {
            if (_vehicles == null || _vehicles.Count == 0) return;

            int nextIndex = _currentVehicleIndex;

            // Keyboard 1-9
            if (Keyboard.current != null)
            {
                if (Keyboard.current.digit1Key.wasPressedThisFrame) nextIndex = 0;
                else if (Keyboard.current.digit2Key.wasPressedThisFrame) nextIndex = 1;
                else if (Keyboard.current.digit3Key.wasPressedThisFrame) nextIndex = 2;
                else if (Keyboard.current.digit4Key.wasPressedThisFrame) nextIndex = 3;
                else if (Keyboard.current.digit5Key.wasPressedThisFrame) nextIndex = 4;
                else if (Keyboard.current.digit6Key.wasPressedThisFrame) nextIndex = 5;
                else if (Keyboard.current.digit7Key.wasPressedThisFrame) nextIndex = 6;
                else if (Keyboard.current.digit8Key.wasPressedThisFrame) nextIndex = 7;
                else if (Keyboard.current.digit9Key.wasPressedThisFrame) nextIndex = 8;
            }

            // Gamepad D-pad
            if (Gamepad.current != null)
            {
                var dpad = Gamepad.current.dpad;
                if (dpad.left.wasPressedThisFrame)
                    nextIndex = (_currentVehicleIndex - 1 + _vehicles.Count) % _vehicles.Count;
                else if (dpad.right.wasPressedThisFrame)
                    nextIndex = (_currentVehicleIndex + 1) % _vehicles.Count;
            }

            if (nextIndex != _currentVehicleIndex && nextIndex >= 0 && nextIndex < _vehicles.Count)
            {
                SwitchVehicle(nextIndex);
            }
        }

        private void SwitchVehicle(int index)
        {
            _vehicles[_currentVehicleIndex].SetControls(_inactiveControls);
            _currentVehicleIndex = index;
            _vehicles[_currentVehicleIndex].SetControls(_activeControls);
        }

        private class NullControls : IControls
        {
            public float Throttle => 0f;
            public float Roll => 0f;
            public float Pitch => 0f;
            public float Yaw => 0f;
            public void Update() { }
        }
    }
}
