using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class AutocannonProjectile : Projectile
    {
        protected override int DirectHitDamage => 3;

        protected override bool HasExplosion => true;

        protected override int ExplosionDamage => 2;

        protected override float ExplosionRadius => 2f;
    }
}
