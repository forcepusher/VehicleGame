using UnityEngine;
using UnityEngine.UI;

namespace BananaParty.VehicleGame
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField]
        private VehicleReference _controlledVehicle;

        [SerializeField]
        private Image _healthBarFill;
        [SerializeField]
        private Text _healthText;

        private void Update()
        {
            if (!_controlledVehicle.IsSet)
                return;

            int health = _controlledVehicle.Value.HealthValue;
            _healthBarFill.fillAmount = (float)health / _controlledVehicle.Value.MaxHealth;
            _healthText.text = health.ToString();
        }   
    }
}
