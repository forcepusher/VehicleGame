using UnityEngine;

namespace BananaParty.VehicleGame
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        protected abstract int DirectHitDamage { get; }
        protected abstract float ExplosionRadius { get; }
        protected virtual int ExplosionDamage => 0;
        protected virtual bool HasExplosion => false;

        [SerializeField]
        private GameObject _explosionEffect;

        private void Awake()
        {
            Destroy(gameObject, 10f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            ApplyDirectHitDamage(collision.collider);
            if (HasExplosion)
            {
                Explode();
            }
            Destroy(gameObject);
        }

        private void ApplyDirectHitDamage(Collider collider)
        {
            if (collider.TryGetComponent<IHealth>(out var health))
            {
                health.TakeDamage(DirectHitDamage);
            }
        }

        private void Explode()
        {
            if (_explosionEffect != null)
            {
                Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<IHealth>(out var health))
                {
                    health.TakeDamage(ExplosionDamage);
                }
            }
        }

        public void ApplyMovement(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }
    }
}
