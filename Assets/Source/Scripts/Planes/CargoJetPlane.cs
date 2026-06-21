using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class CargoJetPlane : JetPlane
    {
        protected override float ParkedVelocity => 0f;
        protected override float TaxiVelocity => 15f;
        protected override float FlightVelocity => 30f;

        // Linear acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        protected override float AccelerationParked => 1.5f;
        protected override Vector3 AngularAccelerationParked => new Vector3(0f, 0f, 0f); // pitch: yaw: roll

        protected override float AccelerationTaxi => 3f;
        protected override Vector3 AngularAccelerationTaxi => new Vector3(0f, 0.5f, 0f); // pitch: yaw: roll

        protected override float AccelerationFight => 10f;
        protected override Vector3 AngularAccelerationFlight => new Vector3(0.5f, 0.5f, 0.5f); // pitch: yaw: roll

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        protected override Vector3 DragParked => new Vector3(0.0f, 0.0f, 0.0f);
        protected override Vector3 AngularDragParked => new Vector3(0.0f, 0.0f, 0.0f); // pitch: yaw: roll

        protected override Vector3 DragTaxi => new Vector3(0.05f, 0.05f, 0.005f);
        protected override Vector3 AngularDragTaxi => new Vector3(0.5f, 0.5f, 0.5f); // pitch: yaw: roll

        protected override Vector3 DragFlight => new Vector3(0.5f, 0.75f, 0.005f);
        protected override Vector3 AngularDragFlight => new Vector3(3.0f, 3.0f, 3.0f); // pitch: yaw: roll
    }
}
