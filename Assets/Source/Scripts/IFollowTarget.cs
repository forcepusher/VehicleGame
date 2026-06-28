using UnityEngine;

namespace BananaParty.VehicleGame
{
    public interface IFollowTarget : IHealth
    {
        Transform FollowTransformFirstPerson { get; }

        Transform FollowTransformThirdPerson { get; }

        Transform FollowTransformBackView { get; }

        Vector3 FollowVelocity { get; }
    }
}
