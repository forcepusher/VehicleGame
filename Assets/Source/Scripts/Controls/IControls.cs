namespace Igrushka.VehicleGame
{
    public interface IControls
    {
        public float Throttle { get; }
        public float Turn { get; }
        public float Pitch { get; }
        public float Yaw { get; }
    }
}
