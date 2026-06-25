using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPoint
{
    Vector3 Position { get; }
    Quaternion Rotation { get; }

    List<string> Vehicles { get; }
}
