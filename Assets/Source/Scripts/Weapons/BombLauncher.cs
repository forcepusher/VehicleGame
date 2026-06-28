using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class BombLauncher : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private Rigidbody _vehicleRigidbody;

        [SerializeField]
        private Projectile _projectilePrefab;
        [SerializeField]
        private Transform _projectileSpawnPoint;
        [SerializeField]
        private AudioSource _firingAudioSource;

        private const int MaxAmmo = 4;
        private const float FireInterval = 0.5f;

        private bool _isFiringVolley;
        private IControls _controls;

        private void FixedUpdate()
        {
            if (_controls.FirePrimary && !_isFiringVolley)
            {
                _isFiringVolley = true;
                StartCoroutine(FireVolley());
            }
        }

        private System.Collections.IEnumerator FireVolley()
        {
            for (int i = 0; i < MaxAmmo; i++)
            {
                Fire();
                _firingAudioSource.Play();

                yield return new WaitForSeconds(FireInterval);
            }

            _isFiringVolley = false;
        }

        private void Fire()
        {
            Projectile projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
            projectile.ApplyMovement(_vehicleRigidbody.linearVelocity);
        }

        public void SetControls(IControls controls)
        {
            _controls = controls;
        }
    }
}
