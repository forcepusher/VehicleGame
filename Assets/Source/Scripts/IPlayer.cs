namespace BananaParty.VehicleGame
{
    public interface IPlayer : IControls
    {
        IVehicle ControlledVehicle { get; }
        void SetControlledVehicle(IVehicle vehicle);
    }
}
