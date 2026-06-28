using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class MainCameraEffects : MonoBehaviour
    {
        [SerializeField]
        private MainCamera _mainCamera;

        [SerializeField]
        private ParticleSystem _ashesParticleSystem;

        private void Update()
        {
            Vector3 followedObjectVelocity = Vector3.zero;
            if (_mainCamera.FollowTarget != null)
                followedObjectVelocity = _mainCamera.FollowTarget.FollowVelocityGlobalSpace;

            SetVelocityFeelParticleSpeed(followedObjectVelocity);
        }

        private void SetVelocityFeelParticleSpeed(Vector3 speed)
        {
            Vector3 worldParticleVelocity = -speed * 0.3f;
            Vector3 particleVelocity = Quaternion.Inverse(transform.rotation) * worldParticleVelocity;
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _ashesParticleSystem.velocityOverLifetime;
            velocityOverLifetime.x = particleVelocity.x;
            velocityOverLifetime.y = particleVelocity.y;
            velocityOverLifetime.z = particleVelocity.z;
        }
    }
}
