using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class PlayRandomSound : MonoBehaviour
    {
        [SerializeField]
        private List<AudioClip> _audioClips;

        private void Start()
        {
            AudioClip randomClip = _audioClips[Random.Range(0, _audioClips.Count)];
            AudioSource.PlayClipAtPoint(randomClip, transform.position);
        }
    }
}
