using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.stickyCursorLock = false;
#endif
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
                Cursor.lockState = CursorLockMode.None;
#endif

            if (Mouse.current?.leftButton.wasPressedThisFrame == true && !IsPointerOverUi())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.visible = true;
        }

        private bool IsPointerOverUi()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null || Mouse.current == null)
                return false;

            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = Mouse.current.position.ReadValue()
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, raycastResults);
            return raycastResults.Count > 0;
        }
    }
}
