namespace BananaParty.VehicleGame
{
    public interface IHealth
    {
        public int MaxHealth { get; }
        public int HealthValue { get; }
        public void TakeDamage(int damage);
    }
}
