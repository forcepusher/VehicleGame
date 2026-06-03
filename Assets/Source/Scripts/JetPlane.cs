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

        // X - Left/Right, Y - Down/Up, Z - Back/Forward
        private Vector3 _linearForceLowVelocity = new Vector3(0f, 0f, 10f);
        // X - Pitch, Y - Yaw, Z - Roll
        private Vector3 _angularForceLowVelocity = new Vector3(0.5f, 0.5f, 0.5f);

        // X - Left/Right, Y - Down/Up, Z - Back/Forward
        private Vector3 _linearDragLowVelocity = new Vector3(0.1f, 0.3f, 0.01f);
        // X - Pitch, Y - Yaw, Z - Roll
        private Vector3 _angularDragLowVelocity = new Vector3(0.5f, 50f, 0.5f);




        public Vector3 PositionOffset => new Vector3(0, 2, -5);
        public Quaternion RotationOffset => Quaternion.identity;

        private void Update()
        {
            _controls.Update();
        }

        private void FixedUpdate()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);
            _rigidbody.AddRelativeForce(-new Vector3(
                localVelocity.x * Mathf.Abs(localVelocity.x) * _linearDragLowVelocity.x,
                localVelocity.y * Mathf.Abs(localVelocity.y) * _linearDragLowVelocity.y,
                localVelocity.z * Mathf.Abs(localVelocity.z) * _linearDragLowVelocity.z
            ), ForceMode.Acceleration);

            Vector3 localAngularVelocity = transform.InverseTransformDirection(_rigidbody.angularVelocity);
            _rigidbody.AddRelativeTorque(-new Vector3(
                localAngularVelocity.x * Mathf.Abs(localAngularVelocity.x) * _angularDragLowVelocity.x,
                localAngularVelocity.y * Mathf.Abs(localAngularVelocity.y) * _angularDragLowVelocity.y,
                localAngularVelocity.z * Mathf.Abs(localAngularVelocity.z) * _angularDragLowVelocity.z
            ), ForceMode.Acceleration);

            _rigidbody.AddRelativeForce(_controls.Throttle * _linearForceLowVelocity * Vector3.forward, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(_angularForceLowVelocity.x * _controls.Pitch * -Vector3.right, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(_angularForceLowVelocity.y * _controls.Yaw * Vector3.up, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(_angularForceLowVelocity.z * _controls.Turn * -Vector3.forward, ForceMode.Acceleration);
        }
    }
}
