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
            vehicle.SetControls(_playerControls);
            _mainCamera.SetFollowTarget(ControlledVehicle);
            _mainCamera.SetControls(_playerControls);
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
