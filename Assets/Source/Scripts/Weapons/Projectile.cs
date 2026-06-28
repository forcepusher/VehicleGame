using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private int _directHitDamage = 50;
        [SerializeField]
        private int _explosionDamage = 200;
        [SerializeField]
        private float _explosionRadius = 5;

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
