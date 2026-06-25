namespace BananaParty.VehicleGame
{
    public interface IHealth
    {
        public bool IsDead { get; }
        public int MaxHealth { get; }
        public int HealthValue { get; }
        public void TakeDamage(int damage);
    }
}
