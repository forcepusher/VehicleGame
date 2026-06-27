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
        [SerializeField]
        private float _fireRate = 5f;

        [SerializeField]
        private AudioSource _firingAudioSource;

        private float _fireCooldown;

        private IControls _controls;

        private void Update()
        {
            if (_fireCooldown > 0)
            {
                _fireCooldown -= Time.deltaTime;
            }

            if (_controls.FirePrimary && _fireCooldown <= 0)
            {
                _fireCooldown = 1f / _fireRate;
                Fire();
            }

            if (_controls.FirePrimary && !_firingAudioSource.isPlaying)
            {
                _firingAudioSource.loop = true;
                _firingAudioSource.Play();
            }

            if (!_controls.FirePrimary && _firingAudioSource.isPlaying)
            {
                _firingAudioSource.loop = false;
                //_firingAudioSource.Stop();
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
