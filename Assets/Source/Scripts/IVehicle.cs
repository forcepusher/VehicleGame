namespace BananaParty.VehicleGame
{
    public interface IVehicle : IFollowTarget
    {
        void SetControls(IControls controls);
    }
}
