using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void PointerLockTracker_Init();

        [DllImport("__Internal")]
        private static extern int PointerLockTracker_CanRequestLock();
#endif

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.stickyCursorLock = false;
            PointerLockTracker_Init();
#endif
        }

        private void Update()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!CanRequestLock())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
                Cursor.lockState = CursorLockMode.None;
#endif

            if (Mouse.current?.leftButton.wasPressedThisFrame == true
                && !IsPointerOverUi()
                && CanRequestLock()
                && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.visible = true;
        }

        private bool CanRequestLock()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PointerLockTracker_CanRequestLock() != 0;
#else
            return true;
#endif
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
