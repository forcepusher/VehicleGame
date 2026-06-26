using System;
using System.Collections.Generic;
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
        private int _lastSoftIndex;
        private int _lastHardIndex;

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

        public void PlayCollisionSound(int damage)
        {
            if (damage <= 1)
                return;

            var sounds = damage > 15 ? _collisionSoundsHard : _collisionSoundsSoft;
            if (sounds.Count == 0)
                throw new InvalidOperationException("Sound array is empty");

            int index = 0;
            if (sounds.Count > 1)
            {
                int lastIndex = damage > 15 ? _lastHardIndex : _lastSoftIndex;
                index = (lastIndex + Random.Range(1, sounds.Count)) % sounds.Count;

                if (damage > 15)
                    _lastHardIndex = index;
                else
                    _lastSoftIndex = index;
            }

            _collisionAudioSource.PlayOneShot(sounds[index]);
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
