using UnityEngine;

namespace BananaParty.VehicleGame
{
    public interface IFollowTarget
    {
        Vector3 FollowPositionFirstPerson { get; }

        Vector3 FollowPositionThirdPerson { get; }

        Quaternion FollowRotation { get; }

        Vector3 FollowVelocity { get; }
    }
}
