using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace BananaParty.VehicleGame
{
    public class JetPlaneSounds : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _engineAudioSource;
        [SerializeField]
        private AudioSource _airAudioSource;

        [SerializeField]
        private AnimationCurve _engineVolumeCurve;
        [SerializeField]
        private AnimationCurve _airVolumeCurve;
        [SerializeField]
        private AnimationCurve _airPitchCurve;

        public bool IsEngineRunning { get; private set; }

        public void UpdateVelocity(float velocity)
        {
            _engineAudioSource.volume = _engineVolumeCurve.Evaluate(velocity);
            _airAudioSource.volume = _airVolumeCurve.Evaluate(velocity);
            _airAudioSource.pitch = _airPitchCurve.Evaluate(velocity);
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
