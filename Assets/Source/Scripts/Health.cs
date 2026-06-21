using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Health : MonoBehaviour
    {
        public int Value { get; private set; } = 100;

        private void OnCollisionEnter(Collision collision)
        {
            int damage = 0;
            Debug.Log(damage);
        }
    }
}
