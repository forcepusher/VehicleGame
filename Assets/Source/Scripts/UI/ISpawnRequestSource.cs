namespace BananaParty.VehicleGame
{
    public interface ISpawnRequestSource
    {
        bool IsSpawnRequested { get; }
        string SelectedVehicleName { get; }
        void ConfirmSpawn();
    }
}
