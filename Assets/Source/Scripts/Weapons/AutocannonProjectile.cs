using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class AutocannonProjectile : Projectile
    {
        [SerializeField]
        private int _directHitDamage = 3;

        protected override int DirectHitDamage => _directHitDamage;

        protected sealed override float ExplosionRadius => 2f;
    }
}
