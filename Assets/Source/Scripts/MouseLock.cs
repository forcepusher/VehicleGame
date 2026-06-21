using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private void Update()
        {
            if (Mouse.current?.leftButton.wasPressedThisFrame == true && !EventSystem.current.IsPointerOverGameObject())
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
