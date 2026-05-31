using UnityEngine;

namespace Igrushka.VehicleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class JetPlane : MonoBehaviour
    {
        [Header("Engine Settings")]
        [SerializeField]
        private float _maxThrust = 100f;
        [SerializeField]
        private float _acceleration = 10f;
        [SerializeField]
        private float _maxSpeed = 200f;

        [Header("Flight Dynamics")]
        [SerializeField]
        private float _liftCoefficient = 0.5f;
        [SerializeField]
        private float _dragCoefficient = 0.01f;
        [SerializeField]
        private float _angularDrag = 2f;

        [Header("Control Sensitivity")]
        [SerializeField]
        private float _pitchSensitivity = 50f;
        [SerializeField]
        private float _rollSensitivity = 80f;
        [SerializeField]
        private float _yawSensitivity = 30f;

        [SerializeField]
        private Rigidbody _rigidbody;

        private float _currentThrust = 0f;
        private float _pitchInput;
        private float _rollInput;
        private float _yawInput;
        private float _throttleInput;

        private void Awake()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _rigidbody.useGravity = true;
            _rigidbody.angularDamping = _angularDrag;
        }

        private void Update()
        {
            // Input handling
            _pitchInput = Input.GetAxis("Vertical"); // W/S or Arrow Up/Down
            _rollInput = Input.GetAxis("Horizontal"); // A/D or Arrow Left/Right
            _yawInput = Input.GetAxis("Yaw"); // Custom axis or Q/E
            _throttleInput = Input.GetAxis("Throttle"); // Custom axis or Shift/Ctrl

            // Manual throttle control if custom axes are not defined
            if (Input.GetKey(KeyCode.LeftShift)) _throttleInput = 1f;
            else if (Input.GetKey(KeyCode.LeftControl)) _throttleInput = -1f;
            else _throttleInput = 0f;
        }

        private void FixedUpdate()
        {
            ApplyThrust();
            ApplyLift();
            ApplyControls();
            ApplyDrag();
        }

        private void ApplyThrust()
        {
            // Smoothly interpolate thrust
            float targetThrust = _throttleInput * _maxThrust;
            _currentThrust = Mathf.MoveTowards(_currentThrust, targetThrust, _acceleration * Time.fixedDeltaTime);

            _rigidbody.AddForce(transform.forward * _currentThrust, ForceMode.Force);
        }

        private void ApplyLift()
        {
            // Lift is based on forward velocity
            float forwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
            float liftForce = forwardSpeed * _liftCoefficient;

            // Lift acts upwards relative to the plane's orientation
            _rigidbody.AddForce(transform.up * liftForce, ForceMode.Force);
        }

        private void ApplyControls()
        {
            // Pitch: Rotation around local X axis
            _rigidbody.AddRelativeTorque(Vector3.right * _pitchInput * _pitchSensitivity, ForceMode.Acceleration);

            // Roll: Rotation around local Z axis
            _rigidbody.AddRelativeTorque(Vector3.forward * -_rollInput * _rollSensitivity, ForceMode.Acceleration);

            // Yaw: Rotation around local Y axis
            _rigidbody.AddRelativeTorque(Vector3.up * _yawInput * _yawSensitivity, ForceMode.Acceleration);
        }

        private void ApplyDrag()
        {
            // Simple aerodynamic drag to cap top speed
            Vector3 velocity = _rigidbody.linearVelocity;
            float speed = velocity.magnitude;

            if (speed > 0)
            {
                Vector3 dragForce = -velocity.normalized * speed * speed * _dragCoefficient;
                _rigidbody.AddForce(dragForce, ForceMode.Force);
            }

            // Clamp speed
            if (speed > _maxSpeed)
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _maxSpeed;
            }
        }
    }
}
