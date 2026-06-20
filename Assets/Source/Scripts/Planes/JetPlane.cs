using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class JetPlane : MonoBehaviour, IFollowTarget
    {
        [SerializeField]
        private Transform _followTransform;

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private Transform _centerOfMass;

        [SerializeField]
        private List<WheelCollider> _wheelColliders;

        [SerializeField]
        private JetPlaneSounds _sounds;

        IControls _controls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

        // Debug output variables
        private float _debugVelocity;
        private Vector3 _debugLinearForce;
        private Vector3 _debugAngularForce;
        private Vector3 _debugLinearDrag;
        private Vector3 _debugAngularDrag;

        protected abstract float ParkedVelocity { get; }
        protected abstract float TaxiVelocity { get; }
        protected abstract float TakeoffVelocity { get; }
        protected abstract float FlightVelocity { get; }

        // Linear: x,y unused; z=thrust acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        protected abstract Vector3 AccelerationParked { get; }
        protected abstract Vector3 AngularAccelerationParked { get; }

        protected abstract Vector3 AccelerationTaxi { get; }
        protected abstract Vector3 AngularAccelerationTaxi { get; }

        protected abstract Vector3 AccelerationTakeoff { get; }
        protected abstract Vector3 AngularAccelerationTakeoff { get; }

        protected abstract Vector3 AccelerationFight { get; }
        protected abstract Vector3 AngularAccelerationFlight { get; }

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        // Angular drag: yaw highest for directional stability, roll lowest for responsiveness.
        protected abstract Vector3 DragParked { get; }
        protected abstract Vector3 AngularDragParked { get; }

        protected abstract Vector3 DragTaxi { get; }
        protected abstract Vector3 AngularDragTaxi { get; }

        protected abstract Vector3 DragTakeoff { get; }
        protected abstract Vector3 AngularDragTakeoff { get; }

        protected abstract Vector3 DragFlight { get; }
        protected abstract Vector3 AngularDragFlight { get; }

        private Vector3 InterpolateKeyframes(float t, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            if (t <= ParkedVelocity) return v0;
            if (t >= FlightVelocity) return v3;

            if (t < TaxiVelocity)
                return Vector3.Lerp(v0, v1, t / TaxiVelocity);

            if (t < TakeoffVelocity)
                return Vector3.Lerp(v1, v2, (t - TaxiVelocity) / (TakeoffVelocity - TaxiVelocity));

            return Vector3.Lerp(v2, v3, (t - TakeoffVelocity) / (FlightVelocity - TakeoffVelocity));
        }

        private Vector3 GetLinearForce(float velocity) => InterpolateKeyframes(velocity, AccelerationParked, AccelerationTaxi, AccelerationTakeoff, AccelerationFight);
        private Vector3 GetAngularForce(float velocity) => InterpolateKeyframes(velocity, AngularAccelerationParked, AngularAccelerationTaxi, AngularAccelerationTakeoff, AngularAccelerationFlight);
        private Vector3 GetLinearDrag(float velocity) => InterpolateKeyframes(velocity, DragParked, DragTaxi, DragTakeoff, DragFlight);
        private Vector3 GetAngularDrag(float velocity) => InterpolateKeyframes(velocity, AngularDragParked, AngularDragTaxi, AngularDragTakeoff, AngularDragFlight);

        public Vector3 FollowPosition => _followTransform.position;
        public Quaternion FollowRotation => _followTransform.rotation;

        private void Awake()
        {
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
        }

        private void Start()
        {

        }

        private void Update()
        {
            _controls.Update();

            if (_controls.Throttle > 0.1f && !_sounds.IsEngineRunning)
                _sounds.StartEngine();

            _sounds.UpdateVelocity(_rigidbody.linearVelocity.magnitude);

            foreach (WheelCollider wheel in _wheelColliders)
            {
                wheel.motorTorque = Mathf.Abs(_controls.Throttle) > Mathf.Epsilon ? 0.000001f : 0;
            }
        }

        private void FixedUpdate()
        {
            float velocity = _rigidbody.linearVelocity.magnitude;
            Vector3 linearForce = GetLinearForce(velocity);
            Vector3 angularForce = GetAngularForce(velocity);
            Vector3 linearDrag = GetLinearDrag(velocity);
            Vector3 angularDrag = GetAngularDrag(velocity);

            _debugVelocity = velocity;
            _debugLinearForce = linearForce;
            _debugAngularForce = angularForce;
            _debugLinearDrag = linearDrag;
            _debugAngularDrag = angularDrag;

            Vector3 localVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);
            _rigidbody.AddRelativeForce(-new Vector3(
                localVelocity.x * Mathf.Abs(localVelocity.x) * linearDrag.x,
                localVelocity.y * Mathf.Abs(localVelocity.y) * linearDrag.y,
                localVelocity.z * Mathf.Abs(localVelocity.z) * linearDrag.z
            ), ForceMode.Acceleration);

            Vector3 localAngularVelocity = transform.InverseTransformDirection(_rigidbody.angularVelocity);
            _rigidbody.AddRelativeTorque(-new Vector3(
                localAngularVelocity.x * Mathf.Abs(localAngularVelocity.x) * angularDrag.x,
                localAngularVelocity.y * Mathf.Abs(localAngularVelocity.y) * angularDrag.y,
                localAngularVelocity.z * Mathf.Abs(localAngularVelocity.z) * angularDrag.z
            ), ForceMode.Acceleration);

            _rigidbody.AddRelativeForce(_controls.Throttle * linearForce.z * Vector3.forward, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(angularForce.x * _controls.Pitch * -Vector3.right, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(angularForce.y * _controls.Yaw * Vector3.up, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(angularForce.z * _controls.Roll * -Vector3.forward, ForceMode.Acceleration);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10f, 10f, 400f, 500f));
            GUILayout.Label("=== JET PLANE DEBUG ===");
            GUILayout.Label($"Velocity: {_debugVelocity:F2} m/s");
            GUILayout.Label($"Throttle: {_controls.Throttle:F3}");
            GUILayout.Label($"Pitch Input: {_controls.Pitch:F3}");
            GUILayout.Label($"Yaw Input: {_controls.Yaw:F3}");
            GUILayout.Label($"Turn (Roll) Input: {_controls.Roll:F3}");

            GUILayout.Space(10f);
            GUILayout.Label("--- LINEAR FORCE (m/s²) ---");
            GUILayout.Label($"Current: {_debugLinearForce:F2}");

            GUILayout.Space(10f);
            GUILayout.Label("--- ANGULAR FORCE (torque) ---");
            GUILayout.Label($"Current [Pitch,Yaw,Roll]: ({_debugAngularForce.x:F2}, {_debugAngularForce.y:F2}, {_debugAngularForce.z:F2})");

            GUILayout.Space(10f);
            GUILayout.Label("--- LINEAR DRAG ---");
            GUILayout.Label($"Current [X,Y,Z]: ({_debugLinearDrag.x:F3}, {_debugLinearDrag.y:F3}, {_debugLinearDrag.z:F3})");

            GUILayout.Space(10f);
            GUILayout.Label("--- ANGULAR DRAG ---");
            GUILayout.Label($"Current [Pitch,Yaw,Roll]: ({_debugAngularDrag.x:F2}, {_debugAngularDrag.y:F2}, {_debugAngularDrag.z:F2})");

            GUILayout.EndArea();
        }
    }
}
