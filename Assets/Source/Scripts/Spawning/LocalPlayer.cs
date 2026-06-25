namespace BananaParty.VehicleGame
{
    public class LocalPlayer : IControls
    {
        private CompositeControls _playerControls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

        public float Throttle => _playerControls.Throttle;

        public float Roll => _playerControls.Roll;

        public float Pitch => _playerControls.Pitch;

        public float Yaw => _playerControls.Yaw;

        public void Update()
        {
            _playerControls.Update();
        }
    }
}
