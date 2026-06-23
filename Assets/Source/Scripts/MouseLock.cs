using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private bool _wantsLock;

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.stickyCursorLock = false;
#endif
        }

        private void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                UnlockCursor();
                return;
            }

            if (Mouse.current?.leftButton.wasPressedThisFrame == true && !IsPointerOverUi())
                LockCursor();
        }

        private void LockCursor()
        {
            _wantsLock = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UnlockCursor()
        {
            _wantsLock = false;
            Cursor.lockState = CursorLockMode.None;
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
