namespace BananaParty.VehicleGame
{
    public interface ISpawnRequest
    {
        bool IsSpawnRequested { get; }
        string SelectedVehicleName { get; }
        void ConfirmSpawn();
    }
}
