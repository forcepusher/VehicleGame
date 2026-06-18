using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class JetPlaneSounds : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _engineAudioSource;

        [SerializeField]
        private AudioClip _engineStartAudioClip;
        [SerializeField]
        private AudioClip _engineLoopAudioClip;
        [SerializeField]
        private AudioClip _engineStopAudioClip;

        private bool _isEngineRunning;

        public void StartEngine()
        {
            if (_isEngineRunning)
                return;

            _isEngineRunning = true;
            PlayStartClip();
        }

        public void StopEngine()
        {
            if (!_isEngineRunning)
                return;

            _isEngineRunning = false;
            _engineAudioSource.Stop();
            PlayStopClip();
        }

        private void PlayStartClip()
        {
            _engineAudioSource.clip = _engineStartAudioClip;
            _engineAudioSource.PlayOneShot(_engineStartAudioClip);
        }

        private void PlayLoopClip()
        {
            _engineAudioSource.clip = _engineLoopAudioClip;
            _engineAudioSource.loop = true;
            _engineAudioSource.Play();
        }

        private void PlayStopClip()
        {
            if (_engineStopAudioClip != null)
            {
                AudioSource stopSource = gameObject.AddComponent<AudioSource>();
                stopSource.clip = _engineStopAudioClip;
                stopSource.PlayOneShot(_engineStopAudioClip);
                Destroy(stopSource, _engineStopAudioClip.length);
            }
        }

        private void Update()
        {
            if (_isEngineRunning && !_engineAudioSource.isPlaying)
            {
                PlayLoopClip();
            }
        }
    }
}
