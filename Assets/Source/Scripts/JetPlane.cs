using System.Collections.Generic;
using UnityEngine;

namespace Igrushka.VehicleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class JetPlane : MonoBehaviour, IFollowTarget
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        IControls _controls = new CompositeControls(new IControls[] { new KeyboardControls(), new GamepadControls() });

        // x - Left/Right, y - Down/Up, z - Back/Forward, t - Velocitykeyframe
        private Vector4 _linearForceLowVelocity = new Vector4(0f, 0f, 10f, 20f);
        // x - Pitch, y - Yaw, z - Roll, t - Velocitykeyframe
        private Vector4 _angularForceLowVelocity = new Vector4(0.5f, 0.5f, 10f, 20f);

        // x - Left/Right, y - Down/Up, z - Back/Forward, t - Velocitykeyframe
        private Vector4 _linearForceMediumVelocity = new Vector4(0f, 0f, 10f, 100f);
        // x - Pitch, y - Yaw, z - Roll, t - Velocitykeyframe
        private Vector4 _angularForceMediumVelocity = new Vector4(0.5f, 0.5f, 0.5f, 100f);

        // x - Left/Right, y - Down/Up, z - Back/Forward,t - Velocitykeyframe
        private Vector4 _linearForceHighVelocity = new Vector4(0f, 0f, 10f, 600f);
        // x - Pitch, y - Yaw, z - Roll, t - Velocitykeyframe
        private Vector4 _angularForceHighVelocity = new Vector4(0.5f, 0.5f, 0.5f, 600f);


        // x - Left/Right, y - Down/Up, z - Back/Forward, t - Velocitykeyframe
        private Vector4 _linearDragLowVelocity = new Vector4(0.1f, 0.3f, 0.01f, 20f);
        // x - Pitch, y - Yaw, z - Roll, t - Velocitykeyframe
        private Vector4 _angularDragLowVelocity = new Vector4(0.5f, 50f, 0.5f, 20f);

        // x - Left/Right, y - Down/Up, z - Back/Forward, t - Velocitykeyframe
        private Vector4 _linearDragMediumVelocity = new Vector4(0.1f, 0.3f, 0.01f, 100f);
        // x - Pitch, y - Yaw, z - Roll, t - Velocitykeyframe
        private Vector4 _angularDragMediumVelocity = new Vector4(0.5f, 50f, 0.5f, 100f);

        // x - Left/Right, y - Down/Up, z - Back/Forward, t - Velocitykeyframe
        private Vector4 _linearDragHighVelocity = new Vector4(0.1f, 0.3f, 0.01f, 600f);
        // x - Pitch, y - Yaw, z - Roll, t - Velocitykeyframe
        private Vector4 _angularDragHighVelocity = new Vector4(0.5f, 50f, 0.5f, 600f);

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
