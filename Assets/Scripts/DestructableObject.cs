using UnityEngine;

public class DestructableObject : MonoBehaviour, IDamageable
{
    [SerializeField]
    private ParticleSystem DestructionSystem;
    [SerializeField]
    private int _Health;
    [SerializeField]
    private int _MaxHealth = 25;
    public int CurrentHealth { get => _Health; private set => _Health = value; }
    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Instantiate(DestructionSystem, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
