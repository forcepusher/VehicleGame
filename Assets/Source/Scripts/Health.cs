using UnityEngine;

namespace BananaParty.VehicleGame
{
    public class Health : MonoBehaviour
    {
        private const float _collisionDamageMultiplier = 1f;

        public int Value { get; private set; } = 100;

        private void OnCollisionEnter(Collision collision)
        {
            int damage = Mathf.RoundToInt(collision.relativeVelocity.magnitude * _collisionDamageMultiplier);
            Value -= damage;
            Value = Mathf.Max(0, Value);
            Debug.Log($"Took {damage} damage. Current health: {Value}");
        }
    }
}
