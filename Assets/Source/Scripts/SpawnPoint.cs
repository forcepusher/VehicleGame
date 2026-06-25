using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class SpawnPoint : MonoBehaviour, ISpawnPoint
    {
        public Vector3 Position => transform.position;

        public Quaternion Rotation => transform.rotation;

        public List<string> Vehicles => new()
        {
            "BomberJetPlane",
            "CargoJetPlane"
        };
    }
}
