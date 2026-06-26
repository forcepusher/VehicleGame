namespace BananaParty.VehicleGame
{
    public interface IPlayer
    {
        IVehicle ControlledVehicle { get; }
        void SetControlledVehicle(IVehicle vehicle);
        void ManualUpdate();
    }
}
