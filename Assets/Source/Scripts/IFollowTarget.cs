using UnityEngine;

namespace BananaParty.VehicleGame
{
    public interface IFollowTarget
    {
        Vector3 FollowPosition { get; }

        Quaternion FollowRotation { get; }

        Vector3 FollowVelocity { get; }
    }
}
