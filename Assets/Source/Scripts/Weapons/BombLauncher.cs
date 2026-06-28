using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class BombLauncher : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private Rigidbody _vehicleRigidbody;

        [SerializeField]
        private BombProjectile _projectilePrefab;
        [SerializeField]
        private Transform _projectileSpawnPoint;
        [SerializeField]
        private AudioSource _firingAudioSource;

        [SerializeField]
        private AudioSource _reloadAudioSource;

        private const int MaxAmmo = 4;
        private const float FireInterval = 0.5f;
        private const float ReloadTime = 2f;

        private int _currentAmmo;
        private float _fireCooldown;
        private float _refillCooldown;
        private bool _isReloading;
        private bool _isFiringVolley;
        private IControls _controls;

        private void Awake()
        {
            _currentAmmo = MaxAmmo;
        }

        private void Update()
        {
            if (_controls.FireSecondary && !_isFiringVolley && _currentAmmo > 0)
            {
                _isFiringVolley = true;
            }
        }

        private void FixedUpdate()
        {
            if (!_isFiringVolley)
            {
                Refill();
                return;
            }

            if (_currentAmmo <= 0 || _fireCooldown > 0)
            {
                if (_currentAmmo <= 0 && !_isReloading)
                {
                    _isReloading = true;
                    _refillCooldown = ReloadTime;
                }

                _isFiringVolley = false;
                return;
            }

            Fire();
            _firingAudioSource.Play();
            _currentAmmo--;
            _fireCooldown = FireInterval;
        }

        private void Fire()
        {
            Projectile projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
            projectile.ApplyMovement(_vehicleRigidbody.linearVelocity);
        }

        private void Refill()
        {
            if (!_isReloading) return;

            if (_refillCooldown > 0)
            {
                _refillCooldown -= Time.fixedDeltaTime;
                return;
            }

            _currentAmmo = MaxAmmo;
            _reloadAudioSource.Play();
            _isReloading = false;
        }

        public void SetControls(IControls controls)
        {
            _controls = controls;
        }
    }
}
