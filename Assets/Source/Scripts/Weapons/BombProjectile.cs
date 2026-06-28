using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class BombProjectile : Projectile
    {
        [SerializeField]
        private int _directHitDamage = 50;

        [SerializeField]
        private int _explosionDamage = 200;

        [SerializeField]
        private float _explosionRadius = 5f;

        protected override int DirectHitDamage => _directHitDamage;

        protected sealed override float ExplosionRadius => _explosionRadius;

        protected sealed override int ExplosionDamage => _explosionDamage;

        protected sealed override bool HasExplosion => true;
    }
}
