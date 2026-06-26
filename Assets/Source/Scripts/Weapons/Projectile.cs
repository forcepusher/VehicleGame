using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float DirectHitDamage = 5;
        [SerializeField]
        private float ExplosionDamage = 5;
        [SerializeField]
        private float ExplosionRadius = 2;

        private void OnCollisionEnter(Collision collision)
        {
            
        }
    }
}
