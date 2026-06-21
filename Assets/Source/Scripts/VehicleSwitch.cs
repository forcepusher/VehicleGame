using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace BananaParty.VehicleGame
{
    public class VehicleSwitch : MonoBehaviour
    {
        [SerializeField]
        VehicleReference _controlledVehicle;

        [SerializeField]
        private MainCamera _mainCamera;
        [SerializeReference]
        private List<GameObject> _vehicleGameObjects;
        private List<IVehicle> _vehicles;

        private CompositeControls _playerControls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });
        private int _currentVehicleIndex = 0;

        private void Awake()
        {
            _vehicles = new List<IVehicle>();
            foreach (var obj in _vehicleGameObjects)
            {
                var vehicle = obj.GetComponent<IVehicle>();
                if (vehicle == null)
                    throw new InvalidOperationException($"GameObject {obj.name} does not implement IVehicle");

                _vehicles.Add(vehicle);
            }
        }

        private void Start()
        {
            SwitchVehicle(_currentVehicleIndex);
        }

        private void Update()
        {
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
            _vehicles[_currentVehicleIndex].SetControls(new InactiveControls());
            _currentVehicleIndex = index;
            _vehicles[_currentVehicleIndex].SetControls(_playerControls);
            _mainCamera.SetFollowTarget(_vehicles[_currentVehicleIndex]);
            _controlledVehicle.Set(_vehicles[_currentVehicleIndex]);
        }

        public void OnSwitchVehicleButtonClick(string vehicleName)
        {
            for (int i = 0; i < _vehicleGameObjects.Count; i++)
            {
                if (_vehicleGameObjects[i].name == vehicleName)
                {
                    SwitchVehicle(i);
                    return;
                }
            }
            Debug.LogWarning($"Vehicle with name {vehicleName} not found.");
        }
    }
}
