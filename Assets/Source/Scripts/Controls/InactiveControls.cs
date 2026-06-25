namespace BananaParty.VehicleGame
{
    public class InactiveControls : IControls
    {
        public float Throttle => 0f;
        public float Roll => 0f;
        public float Pitch => 0f;
        public float Yaw => 0f;
        public void ManualUpdate() { }
    }
}
