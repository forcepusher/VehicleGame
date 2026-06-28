using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class MainCamera : MonoBehaviour
    {
        public IFollowTarget FollowTarget { get; private set; }

        private IControls _controls;
        private bool _isFirstPerson = true;
        private bool _prevSwitchCamera;

        private void LateUpdate()
        {
            if (FollowTarget == null)
                return;

            // Switch camera on press edge
            if (_controls != null && _controls.SwitchCamera && !_prevSwitchCamera)
                _isFirstPerson = !_isFirstPerson;
            _prevSwitchCamera = _controls != null && _controls.SwitchCamera;

            Transform targetTransform;
            if (FollowTarget.IsDead)
                targetTransform = FollowTarget.FollowTransformThirdPerson;
            else if (_controls != null && _controls.BackViewCamera)
                targetTransform = FollowTarget.FollowTransformBackView;
            else
                targetTransform = _isFirstPerson
                    ? FollowTarget.FollowTransformFirstPerson
                    : FollowTarget.FollowTransformThirdPerson;

            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        public void SetFollowTarget(IFollowTarget followTarget)
        {
            FollowTarget = followTarget;
        }

        public void SetControls(IControls controls)
        {
            _controls = controls;
        }
    }
}
