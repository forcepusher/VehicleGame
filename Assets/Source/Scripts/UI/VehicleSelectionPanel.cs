using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class VehicleSelectionPanel : MonoBehaviour, ISpawnRequestSource
    {
        public bool IsSpawnRequested { get; private set; }

        public string SelectedVehicleName { get; private set; }

        public void ConfirmSpawn()
        {
            IsSpawnRequested = false;
        }

        public void OnVehicleSelectionButtonClick(string vehicleName)
        {
            SelectedVehicleName = vehicleName;
            IsSpawnRequested = true;
        }
    }
}
