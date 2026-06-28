using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class BombProjectile : Projectile
    {
        protected override int DirectHitDamage => 50;

        protected sealed override bool HasExplosion => true;

        protected sealed override int ExplosionDamage => 200;

        protected sealed override float ExplosionRadius => 5;
    }
}
