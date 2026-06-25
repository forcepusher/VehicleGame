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

        public void SetControlledVehicle(IVehicle vehicle)
        {
            ControlledVehicle = vehicle;
            vehicle.SetControls(this);
        }

        public void ManualUpdate()
        {
            _playerControls.ManualUpdate();

            if (ControlledVehicle.IsDead)
            {
                _respawnTimeRemaining -= RespawnCooldown;

                if (_respawnTimeRemaining <= 0)
                {
                    var spawnPoint = UnityEngine.Object.FindAnyObjectByType<SpawnPoint>();

                    //Spawn the damn vehicle
                    // But uuuh, player should control where and when to spawn it
                }
            }
            else
            {
                _respawnTimeRemaining = RespawnCooldown;
            }
        }
    }
}
