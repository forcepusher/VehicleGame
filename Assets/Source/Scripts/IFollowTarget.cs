using UnityEngine;

namespace BananaParty.VehicleGame
{
    public interface IFollowTarget : IHealth
    {
        Vector3 FollowPositionFirstPerson { get; }

        Vector3 FollowPositionThirdPerson { get; }

        Vector3 FollowPositionBackView { get; }

        Quaternion FollowRotation { get; }

        Quaternion BackViewFollowRotation { get; }

        Vector3 FollowVelocity { get; }
    }
}
