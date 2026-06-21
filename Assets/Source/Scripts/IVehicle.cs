namespace BananaParty.VehicleGame
{
    public interface IVehicle : IFollowTarget, IHealth
    {
        void SetControls(IControls controls);
    }
}
