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

        public bool IsEngineRunning { get; private set; }
        private Coroutine _startEngineCoroutine;

        public void StartEngine()
        {
            IsEngineRunning = true;

            _engineAudioSource.clip = _engineLoopAudioClip;
            _engineAudioSource.loop = true;

            _startEngineCoroutine = StartCoroutine(PlayEngineLoopAfterStart());
            _engineAudioSource.PlayOneShot(_engineStartAudioClip);
        }

        private IEnumerator PlayEngineLoopAfterStart()
        {
            float waitTime = _engineStartAudioClip.length;

            yield return new WaitForSeconds(waitTime - Time.deltaTime);

            _engineAudioSource.Play();

            _startEngineCoroutine = null;
        }

        public void StopEngine()
        {
            IsEngineRunning = false;

            if (_startEngineCoroutine != null)
                StopCoroutine(_startEngineCoroutine);

            _engineAudioSource.Stop();
            _engineAudioSource.PlayOneShot(_engineStopAudioClip);
        }
    }
}
