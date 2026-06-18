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

        private bool _isRunning;

        public void StartEngine()
        {
            if (_isRunning) return;
            _isRunning = true;

            _engineAudioSource.PlayOneShot(_engineStartAudioClip);

            _engineAudioSource.clip = _engineLoopAudioClip;
            _engineAudioSource.loop = true;
            _engineAudioSource.PlayScheduled(AudioSettings.dspTime + _engineStartAudioClip.length);
        }

        public void StopEngine()
        {
            if (!_isRunning) return;
            _isRunning = false;

            _engineAudioSource.Stop();
            _engineAudioSource.PlayOneShot(_engineStopAudioClip);
        }
    }
}
