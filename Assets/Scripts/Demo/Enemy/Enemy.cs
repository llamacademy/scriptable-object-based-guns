using UnityEngine;

namespace LlamAcademy.Guns.Demo.Enemy
{
    [DisallowMultipleComponent]
    public class Enemy : MonoBehaviour
    {
        public EnemyHealth Health;
        public EnemyMovement Movement;
        public EnemyPainResponse PainResponse;

        private void Start()
        {
            Health.OnTakeDamage += PainResponse.HandlePain;
            Health.OnDeath += Die;
        }

        private void Die(Vector3 Position)
        {
            Movement.StopMoving();
            PainResponse.HandleDeath();
        }
    }
}