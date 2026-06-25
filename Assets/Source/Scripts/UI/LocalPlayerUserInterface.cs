using UnityEngine;
using UnityEngine.UI;

namespace BananaParty.VehicleGame
{
    public class LocalPlayerUserInterface : MonoBehaviour, IUserInterface
    {
        [SerializeField]
        private Image _healthBarFill;
        [SerializeField]
        private Text _healthText;

        private IVehicle _controlledVehicle;

        public bool IsSpawnRequested { get; private set; }

        public string SelectedVehicleName { get; private set; }

        public void ConfirmSpawn(IVehicle vehicle)
        {
            IsSpawnRequested = false;
            _controlledVehicle = vehicle;
        }

        public void OnVehicleSelectionButtonClick(string vehicleName)
        {
            SelectedVehicleName = vehicleName;
            IsSpawnRequested = true;
        }

        public void Update()
        {
            int health = _controlledVehicle?.HealthValue ?? 0;
            _healthBarFill.fillAmount = health / (_controlledVehicle?.MaxHealth ?? 100);
            _healthText.text = health.ToString();
        }

    }
}
