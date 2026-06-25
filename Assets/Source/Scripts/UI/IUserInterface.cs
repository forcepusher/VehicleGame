namespace BananaParty.VehicleGame
{
    public interface IUserInterface
    {
        bool IsSpawnRequested { get; }
        string SelectedVehicleName { get; }
        void ConfirmSpawn(IVehicle vehicle);
    }
}
