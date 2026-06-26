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
        private float _fireRate = 5f;

        private IControls _controls;

        private void Update()
        {
            if (_controls.FirePrimary)
            {

            }
        }

        private void Fire()
        {
            Projectile projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
            projectile.ApplyMovement(_projectileSpawnPoint.forward * _projectileSpeed);
        }

        public void SetControls(IControls controls)
        {
            _controls = controls;
        }
    }
}
