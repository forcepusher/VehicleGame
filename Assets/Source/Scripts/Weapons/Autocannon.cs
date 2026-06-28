using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Autocannon : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private AutocannonProjectile _projectilePrefab;
        [SerializeField]
        private Transform _projectileSpawnPoint;

        private float _projectileSpeed = 50f;
        private float _fireRate = 5f;

        [SerializeField]
        private AudioSource _firingAudioSource;

        private float _fireCooldown;
        private bool _isFiring;

        private IControls _controls;

        private void FixedUpdate()
        {
            if (_fireCooldown > 0)
            {
                _fireCooldown -= Time.fixedDeltaTime;
            }

            if (_isFiring && _fireCooldown <= 0)
            {
                _fireCooldown = 1f / _fireRate;
                Fire();
            }
        }

        private void Update()
        {
            _isFiring = _controls.FirePrimary;

            if (_isFiring && !_firingAudioSource.isPlaying)
            {
                _firingAudioSource.loop = true;
                _firingAudioSource.Play();
            }

            if (!_isFiring && _firingAudioSource.isPlaying)
            {
                _firingAudioSource.loop = false;
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
