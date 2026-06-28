using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        private int _directHitDamage = 3;
        private int _explosionDamage = 2;
        private float _explosionRadius = 2;

        [SerializeField]
        private GameObject _explosionEffect;

        private void Awake()
        {
            Destroy(gameObject, 10f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            ApplyDirectHitDamage(collision.collider);
            Explode();
            Destroy(gameObject);
        }

        private void ApplyDirectHitDamage(Collider collider)
        {
            if (collider.TryGetComponent<IHealth>(out var health))
            {
                health.TakeDamage(_directHitDamage);
            }
        }

        private void Explode()
        {
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);

            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<IHealth>(out var health))
                {
                    health.TakeDamage(_explosionDamage);
                }
            }
        }

        public void ApplyMovement(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }
    }
}
