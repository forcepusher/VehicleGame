using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Autocannon : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private Projectile _projectilePrefab;
        [SerializeField]
        private Transform _projectileSpawnPoint;
        [SerializeField]
        private float _projectileSpeed = 50f;

        public void Fire()
        {
            if (_projectilePrefab == null || _projectileSpawnPoint == null)
                return;

            Projectile projectile = GameObject.Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = _projectileSpawnPoint.forward * _projectileSpeed;
            }
        }
    }
}
