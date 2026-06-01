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

        public Vector3 PositionOffset => new Vector3(0, 2, -5);
        public Quaternion RotationOffset => Quaternion.identity;

        private void Update()
        {
            _controls.Update();
        }

        private void FixedUpdate()
        {

        }
    }
}
