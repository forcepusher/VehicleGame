using UnityEngine;

namespace BananaParty.VehicleGame
{
    public interface IFollowTarget
    {
        Vector3 PositionOffset { get; }

        Quaternion RotationOffset { get; }
    }
}
