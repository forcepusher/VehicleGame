using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class JetPlaneSounds : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _engineAudioSource;

        [SerializeField]
        private AudioSource _engineLoopAudioSource;

        [SerializeField]
        private AudioClip _engineStartAudioClip;
        [SerializeField]
        private AudioClip _engineLoopAudioClip;
        [SerializeField]
        private AudioClip _engineStopAudioClip;

        private bool _isRunning;
        private System.Collections.IEnumerator _startCoroutine;

        public void StartEngine()
        {
            if (_isRunning) return;
            _isRunning = true;

            _engineAudioSource.clip = _engineStartAudioClip;
            _engineAudioSource.Play();

            _engineLoopAudioSource.clip = _engineLoopAudioClip;
            _engineLoopAudioSource.loop = true;

#if UNITY_WEBGL && !UNITY_EDITOR
            _startCoroutine = StartCoroutine(PlayLoopOnWebGL());
#else
            _engineLoopAudioSource.PlayScheduled(AudioSettings.dspTime + _engineStartAudioClip.length);
#endif
        }

        private System.Collections.IEnumerator PlayLoopOnWebGL()
        {
            yield return new WaitForSeconds(_engineStartAudioClip.length);
            if (_isRunning)
            {
                _engineLoopAudioSource.Play();
            }
        }

        public void StopEngine()
        {
            if (!_isRunning) return;
            _isRunning = false;

            if (_startCoroutine != null)
            {
                StopCoroutine(_startCoroutine);
            }

            _engineAudioSource.Stop();
            _engineLoopAudioSource.Stop();

            _engineAudioSource.PlayOneShot(_engineStopAudioClip);
        }
    }
}
