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

        private Vector3 _drag = new Vector3(0.1f, 0.3f, 0.01f);

        private float _rollForce = 0.5f;
        private float _thrustForce = 10f;
        private float _yawForce = 0.5f;
        private float _pitchForce = 1.0f;

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
                localVelocity.x * Mathf.Abs(localVelocity.x) * _drag.x,
                localVelocity.y * Mathf.Abs(localVelocity.y) * _drag.y,
                localVelocity.z * Mathf.Abs(localVelocity.z) * _drag.z
            ), ForceMode.Acceleration);

            _rigidbody.AddRelativeForce(Vector3.forward * _controls.Throttle * _thrustForce, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(-Vector3.right * _controls.Pitch * _pitchForce, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(Vector3.up * _controls.Yaw * _yawForce, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(-Vector3.forward * _controls.Turn * _rollForce, ForceMode.Acceleration);
        }
    }
}
