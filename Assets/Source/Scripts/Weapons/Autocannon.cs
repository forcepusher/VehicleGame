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

        private void Update()
        {

        }

        private void Fire()
        {
            Projectile projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
            projectile.ApplyMovement(_projectileSpawnPoint.forward * _projectileSpeed);
        }

        public void SetControls(IControls controls)
        {

        }
    }
}
