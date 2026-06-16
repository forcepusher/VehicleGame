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

        // Linear: x,y unused; z=thrust acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        private Vector4 _linearForceLowVelocity = new Vector4(0f, 0f, 7f, 20f); // ~0.7G initial acceleration
        private Vector4 _angularForceLowVelocity = new Vector4(3f, 1.5f, 8f, 20f); // pitch: yaw: roll (minimal at low speed)

        private Vector4 _linearForceMediumVelocity = new Vector4(0f, 0f, 5f, 100f); // ~0.5G with drag
        private Vector4 _angularForceMediumVelocity = new Vector4(36f, 14f, 90f, 100f); // pitch: yaw: roll (2.5:1:6 ratio)

        private Vector4 _linearForceHighVelocity = new Vector4(0f, 0f, 3f, 600f); // ~0.3G at high speed (drag dominated)
        private Vector4 _angularForceHighVelocity = new Vector4(30f, 12f, 75f, 600f); // pitch: yaw: roll (2.5:1:6 ratio)

        // Linear drag per axis; z=forward direction should be lowest (streamlined).
        // Angular drag: yaw highest for directional stability, roll lowest for responsiveness.
        private Vector4 _linearDragLowVelocity = new Vector4(0.3f, 0.3f, 0.08f, 20f);
        private Vector4 _angularDragLowVelocity = new Vector4(2.5f, 4f, 1.5f, 20f); // pitch: yaw: roll

        private Vector4 _linearDragMediumVelocity = new Vector4(0.45f, 0.45f, 0.12f, 100f);
        private Vector4 _angularDragMediumVelocity = new Vector4(5f, 8f, 3f, 100f); // pitch: yaw: roll

        private Vector4 _linearDragHighVelocity = new Vector4(0.7f, 0.7f, 0.2f, 600f);
        private Vector4 _angularDragHighVelocity = new Vector4(8f, 12f, 5f, 600f); // pitch: yaw: roll

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

        private Vector3 GetLinearForce(float velocity) => InterpolateKeyframes(velocity, _linearForceLowVelocity, _linearForceMediumVelocity, _linearForceHighVelocity);
        private Vector3 GetAngularForce(float velocity) => InterpolateKeyframes(velocity, _angularForceLowVelocity, _angularForceMediumVelocity, _angularForceHighVelocity);
        private Vector3 GetLinearDrag(float velocity) => InterpolateKeyframes(velocity, _linearDragLowVelocity, _linearDragMediumVelocity, _linearDragHighVelocity);
        private Vector3 GetAngularDrag(float velocity) => InterpolateKeyframes(velocity, _angularDragLowVelocity, _angularDragMediumVelocity, _angularDragHighVelocity);

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
    }
}
