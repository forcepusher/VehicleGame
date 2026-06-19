using System.Collections;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class JetPlaneSounds : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _engineAudioSource;

        [SerializeField]
        private AudioClip _engineStartLoopStopAudioClip;

        public bool IsEngineRunning { get; private set; }

        public void StartEngine()
        {
            IsEngineRunning = true;

            _engineAudioSource.clip = _engineStartLoopStopAudioClip;
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
