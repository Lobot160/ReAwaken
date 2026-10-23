

interface IHealth
{
    float Health { get; }
    float MaxHealth { get; }
    bool IsDead { get; }

    void TakeDamage(Damage amount);
    void Heal(float amount);

    void OnDeath();
}