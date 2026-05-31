using UnityEngine;

public interface IFollowTarget
{
    Vector3 PositionOffset { get; }
    
    Quaternion RotationOffset { get; }
}
