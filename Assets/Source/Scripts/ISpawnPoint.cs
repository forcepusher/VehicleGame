using System.Collections.Generic;
using BananaParty.VehicleGame;
using UnityEngine;

public interface ISpawnPoint
{
    Vector3 Position { get; }
    Quaternion Rotation { get; }

    List<string> Vehicles { get; }

    IVehicle SpawnVehicle(string vehicleName);
}
