using UnityEngine;

namespace Igrushka.VehicleGame
{
    public interface IFollowTarget
    {
        Vector3 PositionOffset { get; }

        Quaternion RotationOffset { get; }
    }
}
