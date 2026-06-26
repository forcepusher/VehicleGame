using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class JetPlaneSounds : MonoBehaviour
    {
        private const float EngineStopTimeout = 10f;

        [SerializeField]
        private AudioSource _engineAudioSource;
        [SerializeField]
        private AudioSource _airAudioSource;
        [SerializeField]
        private AudioSource _gravelAudioSource;
        [SerializeField]
        private AudioSource _collisionAudioSource;

        [SerializeField]
        private List<AudioClip> _collisionSoundsSoft;
        [SerializeField]
        private List<AudioClip> _collisionSoundsHard;

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
        private float _idleTimer;
        private int _lastSoftIndex;
        private int _lastHardIndex;

        public bool IsEngineRunning { get; private set; }

        public void UpdateVelocity(Vector3 localVelocity, bool grounded, float throttle)
        {
            float velocityMagnitude = localVelocity.magnitude;

            bool isThrottlingForward = throttle > 0.1f;
            bool isThrottlingBackward = throttle < -0.1f && localVelocity.z <= -1f;

            if (isThrottlingForward || isThrottlingBackward)
            {
                _idleTimer = 0f;
                if (!IsEngineRunning)
                    StartEngine();
            }
            else if (IsEngineRunning)
            {
                _idleTimer += Time.deltaTime;
                if (_idleTimer >= EngineStopTimeout)
                    StopEngine();
            }

            float gravelFadeTarget = grounded ? 1f : 0f;
            _gravelFade = Mathf.MoveTowards(_gravelFade, gravelFadeTarget, Time.deltaTime / _gravelFadeDuration);

            if (grounded && !_gravelAudioSource.isPlaying)
            {
                _gravelAudioSource.loop = true;
                _gravelAudioSource.Play();
            }

            if (_gravelFade <= 0f && _gravelAudioSource.isPlaying)
                _gravelAudioSource.Stop();

            _engineAudioSource.volume = _engineVolumeCurve.Evaluate(velocityMagnitude);
            _engineAudioSource.pitch = _enginePitchCurve.Evaluate(velocityMagnitude);
            _airAudioSource.volume = _airVolumeCurve.Evaluate(velocityMagnitude);
            _airAudioSource.pitch = _airPitchCurve.Evaluate(velocityMagnitude);
            _gravelAudioSource.volume = _gravelVolumeCurve.Evaluate(velocityMagnitude) * _gravelFade;
            _gravelAudioSource.pitch = _gravelPitchCurve.Evaluate(velocityMagnitude);
        }

        public void PlayCollisionSound(int damage)
        {
            if (damage <= 1)
                return;

            if (damage > 15)
                PlayHardCollision();
            else
                PlaySoftCollision();
        }

        private void PlaySoftCollision() => PlayRandomClip(_collisionSoundsSoft, ref _lastSoftIndex);
        private void PlayHardCollision() => PlayRandomClip(_collisionSoundsHard, ref _lastHardIndex);

        private void PlayRandomClip(List<AudioClip> clips, ref int lastIndex)
        {
            if (clips == null || clips.Count == 0)
                return;

            int index = 0;
            if (clips.Count > 1)
            {
                index = (lastIndex + UnityEngine.Random.Range(1, clips.Count)) % clips.Count;
                lastIndex = index;
            }

            _collisionAudioSource.PlayOneShot(clips[index]);
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
