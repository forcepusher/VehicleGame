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
        protected override float AccelerationParked => 5f; // ~1.2G strong initial push from standstill
        protected override Vector3 AngularAccelerationParked => new Vector3(0f, 0f, 0f); // pitch: yaw: roll

        protected override float AccelerationTaxi => 5f; // ~0.8G ground roll (clunky)
        protected override Vector3 AngularAccelerationTaxi => new Vector3(0f, 5f, 0f); // pitch: yaw: roll (near zero at low speed)

        protected override float AccelerationFight => 20f; // ~1G at cruise speed (good airspeed buildup)
        protected override Vector3 AngularAccelerationFlight => new Vector3(15f, 15f, 15f); // pitch: yaw: roll (smoother in flight)

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        // Angular drag: yaw highest for directional stability, roll lowest for responsiveness.
        protected override Vector3 DragParked => new Vector3(0.3f, 0f, 0.005f);
        protected override Vector3 AngularDragParked => new Vector3(3f, 5f, 2f); // pitch: yaw: roll

        protected override Vector3 DragTaxi => new Vector3(0.3f, 0f, 0.005f);
        protected override Vector3 AngularDragTaxi => new Vector3(3f, 5f, 2f); // pitch: yaw: roll

        protected override Vector3 DragFlight => new Vector3(0.6f, 0.8f, 0.0075f);
        protected override Vector3 AngularDragFlight => new Vector3(7f, 12f, 4f); // pitch: yaw: roll
    }
}
