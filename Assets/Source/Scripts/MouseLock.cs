using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
#endif

            if (Mouse.current?.leftButton.wasPressedThisFrame == true && !EventSystem.current.IsPointerOverGameObject())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
