using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private void Update()
        {
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
