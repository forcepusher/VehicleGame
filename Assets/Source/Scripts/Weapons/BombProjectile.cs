using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class BombProjectile : Projectile
    {
        protected override int DirectHitDamage => 50;

        protected sealed override bool HasExplosion => true;

        protected sealed override int ExplosionDamage => 200;

        protected sealed override float ExplosionRadius => 5;

        private void FixedUpdate()
        {
            if (_rigidbody.linearVelocity == Vector3.zero) return;

            transform.forward = _rigidbody.linearVelocity.normalized;
        }
    }
}
