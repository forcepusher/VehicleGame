using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BananaParty.VehicleGame
{
    public class MouseLock : MonoBehaviour
    {
        private const float RelockCooldownSeconds = 1.5f;

        private float _lockReleasedAt = float.NegativeInfinity;
        private bool _wasLocked;

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.stickyCursorLock = false;
#endif
        }

        private void Update()
        {
            bool isLocked = Cursor.lockState == CursorLockMode.Locked;

            if (_wasLocked && !isLocked)
                _lockReleasedAt = Time.unscaledTime;

            _wasLocked = isLocked;

#if !UNITY_WEBGL || UNITY_EDITOR
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                _lockReleasedAt = Time.unscaledTime;
                Cursor.lockState = CursorLockMode.None;
            }
#endif

            if (!CanRequestLock())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

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
            return Time.unscaledTime - _lockReleasedAt >= RelockCooldownSeconds;
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
