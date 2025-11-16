using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int current;

    private Rigidbody rb;
    private NavMeshAgent agent;

    void Start()
    {
        current = maxHealth;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // damage + knockback
    public void TakeDamage(int dmg, Vector3 knockbackDir, float knockbackForce)
    {
        current -= dmg;

        // Apply knockback if possible
        if (rb != null)
        {
            if (agent != null) agent.isStopped = true; // pause movement for realism
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            Invoke(nameof(ResumeAgent), 0.2f); // short delay
        }

        if (current <= 0)
        {
            Die();
        }
    }

    // damage only (fallback)
    public void TakeDamage(int dmg)
    {
        TakeDamage(dmg, Vector3.zero, 0f);
    }

    private void ResumeAgent()
    {
        if (agent != null) agent.isStopped = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}