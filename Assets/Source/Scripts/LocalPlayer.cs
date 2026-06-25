namespace BananaParty.VehicleGame
{
    public class LocalPlayer : IPlayer
    {
        private const float RespawnCooldown = 15;

        private Map _map;

        private float _respawnTimeRemaining = 0;

        private CompositeControls _playerControls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

        public IVehicle ControlledVehicle { get; private set; }

        public float Throttle => _playerControls.Throttle;

        public float Roll => _playerControls.Roll;

        public float Pitch => _playerControls.Pitch;

        public float Yaw => _playerControls.Yaw;

        public LocalPlayer(Map map)
        {
            _map = map;
        }

        public void SpawnVehicle()
        {
            IVehicle vehicle = _map.SpawnPoints[0].SpawnVehicle(_map.SpawnPoints[0].Vehicles[0]);
            SetControlledVehicle(vehicle);
        }

        public void SetControlledVehicle(IVehicle vehicle)
        {
            ControlledVehicle = vehicle;
            vehicle.SetControls(this);
        }

        public void ManualUpdate()
        {
            _playerControls.ManualUpdate();

            if (ControlledVehicle == null || ControlledVehicle.IsDead)
            {
                _respawnTimeRemaining -= RespawnCooldown;

                if (_respawnTimeRemaining <= 0)
                    SpawnVehicle();
            }
            else
            {
                _respawnTimeRemaining = RespawnCooldown;
            }
        }
    }
}
