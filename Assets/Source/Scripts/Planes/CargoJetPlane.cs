using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class CargoJetPlane : JetPlane
    {
        protected override float ParkedVelocity => 0f;
        protected override float TaxiVelocity => 10f;
        protected override float TakeoffVelocity => 30f;
        protected override float FlightVelocity => 40f;

        // Linear: x,y unused; z=thrust acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        protected override Vector3 AccelerationParked => new Vector3(0f, 0f, 1.5f);
        protected override Vector3 AngularAccelerationParked => new Vector3(0f, 0f, 0f); // pitch: yaw: roll

        protected override Vector3 AccelerationTaxi => new Vector3(0f, 0f, 3f);
        protected override Vector3 AngularAccelerationTaxi => new Vector3(0f, 0.5f, 0f); // pitch: yaw: roll

        protected override Vector3 AccelerationTakeoff => new Vector3(0f, 0f, 7f);
        protected override Vector3 AngularAccelerationTakeoff => new Vector3(1f, 1f, 1f); // pitch: yaw: roll

        protected override Vector3 AccelerationFight => new Vector3(0f, 0f, 7f);
        protected override Vector3 AngularAccelerationFlight => new Vector3(2f, 2f, 2f); // pitch: yaw: roll

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        protected override Vector3 DragParked => new Vector3(0.0f, 0.0f, 0.0f);
        protected override Vector3 AngularDragParked => new Vector3(0.0f, 0.0f, 0.0f); // pitch: yaw: roll

        protected override Vector3 DragTaxi => new Vector3(0.0f, 0.05f, 0.0f);
        protected override Vector3 AngularDragTaxi => new Vector3(0.0f, 0.0f, 0.0f); // pitch: yaw: roll

        protected override Vector3 DragTakeoff => new Vector3(0.0f, 0.25f, 0.0f); // moderate vertical drag for lift-off
        protected override Vector3 AngularDragTakeoff => new Vector3(0.0f, 0.0f, 0.0f); // pitch: yaw: roll

        protected override Vector3 DragFlight => new Vector3(0.0f, 0.25f, 0.0f);
        protected override Vector3 AngularDragFlight => new Vector3(0.0f, 0.0f, 0.0f); // pitch: yaw: roll
    }
}
