using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
