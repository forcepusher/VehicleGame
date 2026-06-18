using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private IFollowTarget _followTarget;

        private void Awake()
        {
            _followTarget = _target.GetComponent<IFollowTarget>();
        }

        private void LateUpdate()
        {
            transform.rotation = _target.rotation * _followTarget.RotationOffset;
            transform.position = _target.position + transform.rotation * _followTarget.PositionOffset;
        }

        private void OnValidate()
        {
            if (_target == null)
                return;

            IFollowTarget followTarget = _target.GetComponent<IFollowTarget>();

            if (followTarget == null)
            {
                Debug.LogError($"Target {_target} must implement IFollowTarget interface");
                _target = null;
            }
        }
    }
}
