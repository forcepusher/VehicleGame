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
        private float _engineStartAudioLength = 3f;
        [SerializeField]
        private AudioClip _engineLoopAudioClip;
        [SerializeField]
        private AudioClip _engineStopAudioClip;

        public bool IsEngineRunning { get; private set; }
        private Coroutine _startEngineCoroutine;

        public void StartEngine()
        {
            IsEngineRunning = true;

            _startEngineCoroutine = StartCoroutine(PlayEngineStartAndLoop());
        }

        private IEnumerator PlayEngineStartAndLoop()
        {
            _engineAudioSource.loop = false;
            _engineAudioSource.clip = _engineStartAudioClip;
            _engineAudioSource.Play();

            yield return new WaitForSeconds(_engineStartAudioLength);

            _engineAudioSource.clip = _engineLoopAudioClip;
            _engineAudioSource.loop = true;
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
