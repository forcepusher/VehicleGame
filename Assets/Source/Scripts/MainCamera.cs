using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class MainCamera : MonoBehaviour
    {
        private IFollowTarget _followTarget;

        private void LateUpdate()
        {
            if (_followTarget == null)
                return;

            transform.position = _followTarget.FollowPosition;
            transform.rotation = _followTarget.FollowRotation;
        }

        public void SetFollowTarget(IFollowTarget followTarget)
        {
            _followTarget = followTarget;
        }
    }
}
