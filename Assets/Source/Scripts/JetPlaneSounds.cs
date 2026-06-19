using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace BananaParty.VehicleGame
{
    public class JetPlaneSounds : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerSnapshot _lowVelocitySnapshot;
        [SerializeField]
        private AudioMixerSnapshot _highVelocitySnapshot;

        [SerializeField]
        private AudioSource _engineAudioSource;

        [SerializeField]
        private AudioClip _engineStartLoopStopAudioClip;

        public bool IsEngineRunning { get; private set; }

        public void UpdateVelocity(float velocity)
        {
            if (velocity > 10f)
            {
                _highVelocitySnapshot.TransitionTo(2f);
            }
            else
            {
                _lowVelocitySnapshot.TransitionTo(0.1f);
            }
        }

        public void StartEngine()
        {
            IsEngineRunning = true;

            _engineAudioSource.loop = true;
            _engineAudioSource.Play();
        }

        public void StopEngine()
        {
            IsEngineRunning = false;

            _engineAudioSource.loop = false;
        }
    }
}
