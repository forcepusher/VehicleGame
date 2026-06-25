using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class MainCamera : MonoBehaviour
    {
        public IFollowTarget FollowTarget {  get; private set; }

        private void LateUpdate()
        {
            if (FollowTarget == null)
                return;

            if (FollowTarget.IsDead)
                transform.position = FollowTarget.FollowPositionThirdPerson;
            else
                transform.position = FollowTarget.FollowPositionFirstPerson;

            transform.rotation = FollowTarget.FollowRotation;
        }

        public void SetFollowTarget(IFollowTarget followTarget)
        {
            FollowTarget = followTarget;
        }
    }
}
