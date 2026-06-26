using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private int _directHitDamage = 5;
        [SerializeField]
        private int _explosionDamage = 5;
        [SerializeField]
        private float _explosionRadius = 2;

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
