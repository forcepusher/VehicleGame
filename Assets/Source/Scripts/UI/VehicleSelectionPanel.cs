using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class VehicleSelectionPanel : MonoBehaviour, ISpawnRequest
    {
        public bool IsSpawnRequested { get; private set; }

        public string SelectedVehicleName { get; private set; }

        public void ConfirmSpawn()
        {
            IsSpawnRequested = false;
        }
    }
}
