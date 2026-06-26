using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class LocalPlayer : IPlayer
    {
        private Map _map;
        private MainCamera _mainCamera;
        private IUserInterface _spawnRequestSource;

        private CompositeControls _playerControls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

        public IVehicle ControlledVehicle { get; private set; }

        public float Throttle => _playerControls.Throttle;

        public float Roll => _playerControls.Roll;

        public float Pitch => _playerControls.Pitch;

        public float Yaw => _playerControls.Yaw;

        public bool FirePrimary => _playerControls.FirePrimary;

        public bool FireSecondary => _playerControls.FireSecondary;

        public LocalPlayer(Map map, MainCamera mainCamera, IUserInterface spawnRequestSource)
        {
            _map = map;
            _mainCamera = mainCamera;
            _spawnRequestSource = spawnRequestSource;
        }

        public IVehicle SpawnVehicle(string vehicleName)
        {
            IVehicle vehicle = _map.SpawnPoints[0].SpawnVehicle(vehicleName);
            SetControlledVehicle(vehicle);
            return vehicle;
        }

        public void SetControlledVehicle(IVehicle vehicle)
        {
            ControlledVehicle = vehicle;
            vehicle.SetControls(this);
            _mainCamera.SetFollowTarget(ControlledVehicle);
        }

        public void ManualUpdate()
        {
            _playerControls.ManualUpdate();

            if (_spawnRequestSource.IsSpawnRequested)
            {
                if (ControlledVehicle != null)
                {
                    ControlledVehicle.Destroy();
                    ControlledVehicle = null;
                }

                IVehicle vehicle = SpawnVehicle(_spawnRequestSource.SelectedVehicleName);
                _spawnRequestSource.ConfirmSpawn(vehicle);
            }
        }
    }
}
