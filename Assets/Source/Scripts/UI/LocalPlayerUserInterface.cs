using UnityEngine;
using UnityEngine.UI;

namespace BananaParty.VehicleGame
{
    public class LocalPlayerUserInterface : MonoBehaviour, IUserInterface
    {
        private const float AutoSpawnTimeSeconds = 10f;

        [SerializeField]
        private Text _autoSpawnText;
        [SerializeField]
        private Image _healthBarFill;
        [SerializeField]
        private Text _healthText;

        private float _timeRemainingToAutoSpawn = AutoSpawnTimeSeconds;
        private string _initialRespawnText;

        private IVehicle _controlledVehicle;

        public bool IsSpawnRequested { get; private set; }

        public string SelectedVehicleName { get; private set; } = "BomberJetPlane";

        private void Awake()
        {
            _initialRespawnText = _autoSpawnText.text;
        }

        public void Update()
        {
            if (_controlledVehicle == null || _controlledVehicle.IsDead)
            {
                if (!_autoSpawnText.enabled)
                    _autoSpawnText.enabled = true;

                _timeRemainingToAutoSpawn -= Time.deltaTime;
                _autoSpawnText.text = _initialRespawnText + (int)_timeRemainingToAutoSpawn;

                if (_timeRemainingToAutoSpawn <= 0 && !IsSpawnRequested)
                    OnVehicleSelectionButtonClick(SelectedVehicleName);
            }
            else
            {
                if (_autoSpawnText.enabled)
                    _autoSpawnText.enabled = false;

                _timeRemainingToAutoSpawn = AutoSpawnTimeSeconds;
            }

            int health = _controlledVehicle?.HealthValue ?? 0;
            _healthBarFill.fillAmount = (float)health / (_controlledVehicle?.MaxHealth ?? 100);
            _healthText.text = health.ToString();
        }

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
    }
}
