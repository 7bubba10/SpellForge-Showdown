using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int current;

    private void Start()
    {
        current = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        current -= dmg;

        if (current <= 0)
            Die();
    }

    private void Die()
    {
        // Notify GameManager that an enemy has died
        GameManager.Instance.EnemyDead();

        // Destroy the enemy object
        Destroy(gameObject);
    }
}