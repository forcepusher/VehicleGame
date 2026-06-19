using UnityEngine;

namespace BananaParty.VehicleGame
{
    public interface IFollowTarget
    {
        Vector3 FollowPosition { get; }

        Quaternion FollowRotation { get; }
    }
}
