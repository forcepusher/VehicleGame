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
            Vector3 particleVelocity = _mainCamera.FollowTarget.FollowVelocity * 0.25f;
            _ashesParticleSystem.velocityOverLifetime.z = particleVelocity.z;
        }
    }
}
