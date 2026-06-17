using System.Collections.Generic;
using UnityEngine;

namespace Igrushka.VehicleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class JetPlane : MonoBehaviour, IFollowTarget
    {
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private Transform _centerOfMass;

        IControls _controls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

        // Debug output variables
        private float _debugVelocity;
        private Vector3 _debugLinearForce;
        private Vector3 _debugAngularForce;
        private Vector3 _debugLinearDrag;
        private Vector3 _debugAngularDrag;

        // Linear: x,y unused; z=thrust acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        private Vector4 _accelerationParked = new Vector4(0f, 0f, 5f, 0f); // ~0.5G slow ground roll (clunky)
        private Vector4 _angularAccelerationParked = new Vector4(3f, 1.5f, 0.5f, 0f); // pitch: yaw: roll (near zero at low speed)

        private Vector4 _accelerationTaxi = new Vector4(0f, 0f, 5f, 5f); // ~0.5G slow ground roll (clunky)
        private Vector4 _angularAccelerationTaxi = new Vector4(3f, 1.5f, 0.5f, 5f); // pitch: yaw: roll (near zero at low speed)

        private Vector4 _accelerationTakeoff = new Vector4(0f, 0f, 8f, 15f); // ~0.8G for takeoff acceleration
        private Vector4 _angularAccelerationTakeoff = new Vector4(20f, 8f, 50f, 15f); // pitch: yaw: roll (sluggish controls)

        private Vector4 _accelerationFight = new Vector4(0f, 0f, 4f, 100f); // ~0.4G at cruise speed
        private Vector4 _angularAccelerationFlight = new Vector4(35f, 12f, 80f, 100f); // pitch: yaw: roll (responsive in flight)

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        // Angular drag: yaw highest for directional stability, roll lowest for responsiveness.
        private Vector4 _dragParked = new Vector4(0.3f, 0.3f, 0.01f, 0f);
        private Vector4 _angularDragParked = new Vector4(3f, 5f, 2f, 0f); // pitch: yaw: roll

        private Vector4 _dragTaxi = new Vector4(0.3f, 0.3f, 0.01f, 5f);
        private Vector4 _angularDragTaxi = new Vector4(3f, 5f, 2f, 5f); // pitch: yaw: roll

        private Vector4 _dragTakeoff = new Vector4(0.4f, 0.4f, 0.015f, 15f);
        private Vector4 _angularDragTakeoff = new Vector4(4f, 7f, 3f, 15f); // pitch: yaw: roll

        private Vector4 _dragFlight = new Vector4(0.6f, 0.6f, 0.03f, 100f);
        private Vector4 _angularDragFlight = new Vector4(7f, 12f, 4f, 100f); // pitch: yaw: roll

        private Vector3 InterpolateKeyframes(float t, Vector4 low, Vector4 medium, Vector4 high)
        {
            if (t <= low.w) return new Vector3(low.x, low.y, low.z);
            if (t >= high.w) return new Vector3(high.x, high.y, high.z);

            if (t < medium.w)
            {
                float lerpT = (t - low.w) / (medium.w - low.w);
                return Vector3.Lerp(new Vector3(low.x, low.y, low.z), new Vector3(medium.x, medium.y, medium.z), lerpT);
            }

            float lerpT2 = (t - medium.w) / (high.w - medium.w);
            return Vector3.Lerp(new Vector3(medium.x, medium.y, medium.z), new Vector3(high.x, high.y, high.z), lerpT2);
        }

        private Vector3 GetLinearForce(float velocity) => InterpolateKeyframes(velocity, _accelerationTaxi, _accelerationTakeoff, _accelerationFight);
        private Vector3 GetAngularForce(float velocity) => InterpolateKeyframes(velocity, _angularAccelerationTaxi, _angularAccelerationTakeoff, _angularAccelerationFlight);
        private Vector3 GetLinearDrag(float velocity) => InterpolateKeyframes(velocity, _dragTaxi, _dragTakeoff, _dragFlight);
        private Vector3 GetAngularDrag(float velocity) => InterpolateKeyframes(velocity, _angularDragTaxi, _angularDragTakeoff, _angularDragFlight);

        public Vector3 PositionOffset => new Vector3(0, 2, -5);
        public Quaternion RotationOffset => Quaternion.identity;

        private void Awake()
        {
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
        }

        private void Update()
        {
            _controls.Update();
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
            _rigidbody.AddRelativeTorque(angularForce.z * _controls.Turn * -Vector3.forward, ForceMode.Acceleration);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10f, 10f, 400f, 500f));
            GUILayout.Label("=== JET PLANE DEBUG ===");
            GUILayout.Label($"Velocity: {_debugVelocity:F2} m/s");
            GUILayout.Label($"Throttle: {_controls.Throttle:F3}");
            GUILayout.Label($"Pitch Input: {_controls.Pitch:F3}");
            GUILayout.Label($"Yaw Input: {_controls.Yaw:F3}");
            GUILayout.Label($"Turn (Roll) Input: {_controls.Turn:F3}");

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
