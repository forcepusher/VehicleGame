using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Autocannon : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private Projectile _projectilePrefab;
        [SerializeField]
        private Transform _muzzle;
        [SerializeField]
        private float _projectileSpeed = 50f;

        public void Fire()
        {
            if (_projectilePrefab == null || _muzzle == null)
                return;

            Projectile projectile = Instantiate(_projectilePrefab, _muzzle.position, _muzzle.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = _muzzle.forward * _projectileSpeed;
            }
        }
    }
}
