using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class JetPlane : MonoBehaviour, IFollowTarget
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

        private const float _parkedVelocity = 0f;
        private const float _taxiVelocity = 10f;
        private const float _takeoffVelocity = 15f;
        private const float _flightVelocity = 30f;

        // Linear: x,y unused; z=thrust acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        private Vector3 _accelerationParked = new Vector3(0f, 0f, 5f); // ~1.2G strong initial push from standstill
        private Vector3 _angularAccelerationParked = new Vector3(0f, 0f, 0f); // pitch: yaw: roll

        private Vector3 _accelerationTaxi = new Vector3(0f, 0f, 5f); // ~0.8G ground roll (clunky)
        private Vector3 _angularAccelerationTaxi = new Vector3(0f, 5f, 0f); // pitch: yaw: roll (near zero at low speed)

        private Vector3 _accelerationTakeoff = new Vector3(0f, 0f, 8f); // ~0.8G for takeoff acceleration
        private Vector3 _angularAccelerationTakeoff = new Vector3(5f, 10f, 10f); // pitch: yaw: roll (sluggish controls)

        private Vector3 _accelerationFight = new Vector3(0f, 0f, 20f); // ~1G at cruise speed (good airspeed buildup)
        private Vector3 _angularAccelerationFlight = new Vector3(15f, 15f, 15f); // pitch: yaw: roll (smoother in flight)

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        // Angular drag: yaw highest for directional stability, roll lowest for responsiveness.
        private Vector3 _dragParked = new Vector3(0.3f, 0f, 0.005f);
        private Vector3 _angularDragParked = new Vector3(3f, 5f, 2f); // pitch: yaw: roll

        private Vector3 _dragTaxi = new Vector3(0.3f, 0f, 0.005f);
        private Vector3 _angularDragTaxi = new Vector3(3f, 5f, 2f); // pitch: yaw: roll

        private Vector3 _dragTakeoff = new Vector3(0.4f, 0.01f, 0.015f); // moderate vertical drag for lift-off
        private Vector3 _angularDragTakeoff = new Vector3(4f, 7f, 3f); // pitch: yaw: roll

        private Vector3 _dragFlight = new Vector3(0.6f, 0.8f, 0.0075f);
        private Vector3 _angularDragFlight = new Vector3(7f, 12f, 4f); // pitch: yaw: roll

        private Vector3 InterpolateKeyframes(float t, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            if (t <= _parkedVelocity) return v0;
            if (t >= _flightVelocity) return v3;

            if (t < _taxiVelocity)
                return Vector3.Lerp(v0, v1, t / _taxiVelocity);

            if (t < _takeoffVelocity)
                return Vector3.Lerp(v1, v2, (t - _taxiVelocity) / (_takeoffVelocity - _taxiVelocity));

            return Vector3.Lerp(v2, v3, (t - _takeoffVelocity) / (_flightVelocity - _takeoffVelocity));
        }

        private Vector3 GetLinearForce(float velocity) => InterpolateKeyframes(velocity, _accelerationParked, _accelerationTaxi, _accelerationTakeoff, _accelerationFight);
        private Vector3 GetAngularForce(float velocity) => InterpolateKeyframes(velocity, _angularAccelerationParked, _angularAccelerationTaxi, _angularAccelerationTakeoff, _angularAccelerationFlight);
        private Vector3 GetLinearDrag(float velocity) => InterpolateKeyframes(velocity, _dragParked, _dragTaxi, _dragTakeoff, _dragFlight);
        private Vector3 GetAngularDrag(float velocity) => InterpolateKeyframes(velocity, _angularDragParked, _angularDragTaxi, _angularDragTakeoff, _angularDragFlight);

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
