using System.Collections;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class DestroyTimer : MonoBehaviour
    {
        [SerializeField]
        private float _seconds = 4f;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_seconds);
            Destroy(this);
        }
    }
}
