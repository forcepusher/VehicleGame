using UnityEngine;

namespace Igrushka.VehicleGame
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        private IFollowTarget _followTarget;

        private void LateUpdate()
        {
            transform.position = _target.position + _followTarget.PositionOffset;
            transform.rotation = _target.rotation * _followTarget.RotationOffset;
        }

        private void OnValidate()
        {
            if (_target is not IFollowTarget)
            {
                Debug.LogError($"Target {_target} must implement IFollowTarget interface");
                _target = null;
            }
        }
    }
}
