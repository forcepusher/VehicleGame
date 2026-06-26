namespace BananaParty.VehicleGame
{
    public interface IControls
    {
        public float Throttle { get; }
        public float Roll { get; }
        public float Pitch { get; }
        public float Yaw { get; }

        public void ManualUpdate();
    }
}
