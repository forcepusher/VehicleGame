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
        private AudioSource _gravelAudioSource;

        [SerializeField]
        private AnimationCurve _engineVolumeCurve;
        [SerializeField]
        private AnimationCurve _enginePitchCurve;
        [SerializeField]
        private AnimationCurve _airVolumeCurve;
        [SerializeField]
        private AnimationCurve _airPitchCurve;
        [SerializeField]
        private AnimationCurve _gravelVolumeCurve;
        [SerializeField]
        private AnimationCurve _gravelPitchCurve;

        public bool IsEngineRunning { get; private set; }
        private bool _gravelSoundPlaying;


        public void UpdateVelocity(float velocity, bool grounded)
        {
            _engineAudioSource.volume = _engineVolumeCurve.Evaluate(velocity);
            _engineAudioSource.pitch = _enginePitchCurve.Evaluate(velocity);
            _airAudioSource.volume = _airVolumeCurve.Evaluate(velocity);
            _airAudioSource.pitch = _airPitchCurve.Evaluate(velocity);
            _gravelAudioSource.volume = _gravelVolumeCurve.Evaluate(velocity);
            _gravelAudioSource.pitch = _gravelPitchCurve.Evaluate(velocity);

            if (grounded)
            {
                if (!_gravelSoundPlaying)
                {
                    _gravelAudioSource.loop = true;
                    _gravelAudioSource.Play();
                    _gravelSoundPlaying = true;
                }
            }
            else
            {
                if (_gravelSoundPlaying)
                {
                    _gravelAudioSource.loop = false;
                    _gravelSoundPlaying = false;
                }
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
