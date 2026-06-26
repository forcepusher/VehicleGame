using System.Collections.Generic;
using UnityEngine;

namespace BananaParty.VehicleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class JetPlane : MonoBehaviour, IVehicle
    {
        private GUIStyle _debugStyle;

        [SerializeField]
        private Transform _followTransformFirstPerson;
        [SerializeField]
        private Transform _followTransformThirdPerson;

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private Transform _centerOfMass;

        [SerializeField]
        private List<WheelCollider> _wheelColliders;

        [SerializeField]
        private JetPlaneSounds _sounds;

        [SerializeField]
        private GameObject _deathEffects;

        IControls _controls = new InactiveControls();

        public bool IsDead { get; private set; }
        public int MaxHealth => 100;
        public int HealthValue { get; private set; } = 100;
        protected abstract float CollisionDamageMultiplier { get; }


        protected abstract float ParkedVelocity { get; }
        protected abstract float TaxiVelocity { get; }
        protected abstract float FlightVelocity { get; }

        // Linear acceleration (m/s²). Angular: x=pitch torque, y=yaw torque, z=roll torque
        protected abstract float AccelerationParked { get; }
        protected abstract Vector3 AngularAccelerationParked { get; }

        protected abstract float AccelerationTaxi { get; }
        protected abstract Vector3 AngularAccelerationTaxi { get; }

        protected abstract float AccelerationFight { get; }
        protected abstract Vector3 AngularAccelerationFlight { get; }

        // Linear drag per axis; z=forward direction should be lowest (streamlined jet).
        // Angular drag: yaw highest for directional stability, roll lowest for responsiveness.
        protected abstract Vector3 DragParked { get; }
        protected abstract Vector3 AngularDragParked { get; }

        protected abstract Vector3 DragTaxi { get; }
        protected abstract Vector3 AngularDragTaxi { get; }

        protected abstract Vector3 DragFlight { get; }
        protected abstract Vector3 AngularDragFlight { get; }

        private float InterpolateKeyframes(float t, float v0, float v1, float v2)
        {
            if (t <= ParkedVelocity) return v0;
            if (t >= FlightVelocity) return v2;

            if (t < TaxiVelocity)
                return Mathf.Lerp(v0, v1, t / TaxiVelocity);

            return Mathf.Lerp(v1, v2, (t - TaxiVelocity) / (FlightVelocity - TaxiVelocity));
        }

        private Vector3 InterpolateKeyframesVector3(float t, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            if (t <= ParkedVelocity) return v0;
            if (t >= FlightVelocity) return v2;

            if (t < TaxiVelocity)
                return Vector3.Lerp(v0, v1, t / TaxiVelocity);

            return Vector3.Lerp(v1, v2, (t - TaxiVelocity) / (FlightVelocity - TaxiVelocity));
        }

        private float GetLinearForce(float velocity) => InterpolateKeyframes(velocity, AccelerationParked, AccelerationTaxi, AccelerationFight);
        private Vector3 GetAngularForce(float velocity) => InterpolateKeyframesVector3(velocity, AngularAccelerationParked, AngularAccelerationTaxi, AngularAccelerationFlight);
        private Vector3 GetLinearDrag(float velocity) => InterpolateKeyframesVector3(velocity, DragParked, DragTaxi, DragFlight);
        private Vector3 GetAngularDrag(float velocity) => InterpolateKeyframesVector3(velocity, AngularDragParked, AngularDragTaxi, AngularDragFlight);

        public Vector3 FollowPositionFirstPerson => _followTransformFirstPerson.position;
        public Vector3 FollowPositionThirdPerson => _followTransformThirdPerson.position;
        public Quaternion FollowRotation => _followTransformFirstPerson.rotation;
        public Vector3 FollowVelocity => _rigidbody.linearVelocity;

        public void SetControls(IControls controls)
        {
            _controls = controls;

            if (_controls is InactiveControls)
            {
                _sounds.StopEngine();
                _rigidbody.isKinematic = true;
            }
            else
            {
                _rigidbody.isKinematic = false;
            }
        }

        private void Awake()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
            _debugStyle = new GUIStyle();
            _debugStyle.fontSize = 14;
            _debugStyle.normal.textColor = Color.yellow;
            _debugStyle.border = new RectOffset(5, 5, 5, 5);
        }

        private void Update()
        {
            _controls.ManualUpdate();

            bool isGrounded = false;
            foreach (WheelCollider wheelCollider in _wheelColliders)
                if (wheelCollider.isGrounded)
                    isGrounded = true;

            _sounds.UpdateVelocity(_rigidbody.linearVelocity.magnitude, isGrounded, Mathf.Abs(_controls.Throttle) > 0.1f);

            foreach (WheelCollider wheel in _wheelColliders)
                wheel.motorTorque = Mathf.Abs(_controls.Throttle) > Mathf.Epsilon ? 0.000001f : 0;
        }

        private void FixedUpdate()
        {
            float velocity = _rigidbody.linearVelocity.magnitude;
            float linearForce = GetLinearForce(velocity);
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

            _rigidbody.AddRelativeForce(_controls.Throttle * linearForce * Vector3.forward, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(angularForce.x * _controls.Pitch * -Vector3.right, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(angularForce.y * _controls.Yaw * Vector3.up, ForceMode.Acceleration);
            _rigidbody.AddRelativeTorque(angularForce.z * _controls.Roll * -Vector3.forward, ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 velocityChange = collision.impulse / _rigidbody.mass;
            int damage = Mathf.RoundToInt(velocityChange.magnitude * CollisionDamageMultiplier);
            TakeDamage(damage);
            _sounds.PlayCollisionSound(damage);
            Debug.Log($"Took {damage} damage from collision (velocity change {velocityChange.magnitude:F1} m/s). Current health: {HealthValue}");
        }

        public void TakeDamage(int damage)
        {
            HealthValue -= damage;
            HealthValue = Mathf.Max(0, HealthValue);
            if (HealthValue <= 0 && !IsDead)
            {
                IsDead = true;

                Instantiate(_deathEffects, transform.position, Quaternion.identity);

                // Nullify for camera follower effects
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;

                gameObject.SetActive(false);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnGUI()
        {
            if (_controls is InactiveControls)
                return;

            GUI.Label(new Rect(10, 10, 200, 30), "Velocity: " + Mathf.Round(_rigidbody.linearVelocity.magnitude));
        }
    }
}
