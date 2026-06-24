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

        [SerializeField]
        private float _gravelFadeDuration = 0.5f;

        private float _gravelFade;

        public bool IsEngineRunning { get; private set; }

        public void UpdateVelocity(float velocity, bool grounded)
        {
            float gravelFadeTarget = grounded ? 1f : 0f;
            _gravelFade = Mathf.MoveTowards(_gravelFade, gravelFadeTarget, Time.deltaTime / _gravelFadeDuration);

            if (grounded && !_gravelAudioSource.isPlaying)
            {
                _gravelAudioSource.loop = true;
                _gravelAudioSource.Play();
            }

            if (_gravelFade <= 0f && _gravelAudioSource.isPlaying)
                _gravelAudioSource.Stop();

            _engineAudioSource.volume = _engineVolumeCurve.Evaluate(velocity);
            _engineAudioSource.pitch = _enginePitchCurve.Evaluate(velocity);
            _airAudioSource.volume = _airVolumeCurve.Evaluate(velocity);
            _airAudioSource.pitch = _airPitchCurve.Evaluate(velocity);
            _gravelAudioSource.volume = _gravelVolumeCurve.Evaluate(velocity) * _gravelFade;
            _gravelAudioSource.pitch = _gravelPitchCurve.Evaluate(velocity);
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
