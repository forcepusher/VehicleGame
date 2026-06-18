using System.Collections;
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
        private Coroutine _startCoroutine;

        public void StartEngine()
        {
            if (_isRunning) return;
            _isRunning = true;

            _engineAudioSource.clip = _engineLoopAudioClip;
            _engineAudioSource.loop = true;

            _startCoroutine = StartCoroutine(PlayLoopAfterStart());
            _engineAudioSource.PlayOneShot(_engineStartAudioClip);
        }

        private IEnumerator PlayLoopAfterStart()
        {
            float waitTime = _engineStartAudioClip.length;

            yield return new WaitForSeconds(waitTime);

            _engineAudioSource.Play();
        }

        public void StopEngine()
        {
            if (!_isRunning) return;
            _isRunning = false;

            if (_startCoroutine != null)
                StopCoroutine(_startCoroutine);

            _engineAudioSource.Stop();
            _engineAudioSource.PlayOneShot(_engineStopAudioClip);
        }
    }
}
