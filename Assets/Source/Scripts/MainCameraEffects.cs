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
            if (_mainCamera.FollowTarget == null)
                return;

            IFollowTarget followTarget = _mainCamera.FollowTarget;
            Vector3 worldParticleVelocity = -followTarget.FollowVelocity * 0.3f;
            Vector3 particleVelocity = Quaternion.Inverse(followTarget.FollowRotation) * worldParticleVelocity;
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _ashesParticleSystem.velocityOverLifetime;
            velocityOverLifetime.x = particleVelocity.x;
            velocityOverLifetime.y = particleVelocity.y;
            velocityOverLifetime.z = particleVelocity.z;
        }
    }
}
