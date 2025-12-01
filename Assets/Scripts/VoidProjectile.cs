using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class VoidProjectile : MonoBehaviour
{
    [Header("Base Stats")]
    public int damage = 20;
    public float speed = 25f;
    public GameObject origin;

    [Header("Chain Settings")]
    public int maxBounces = 5;
    public float bounceRange = 12f;

    private Rigidbody rb;
    private int currentBounce = 0;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        // --------- DAMAGE ENEMY -----------
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            if (!hitEnemies.Contains(other))
            {
                enemy.TakeDamage(damage);
                hitEnemies.Add(other);
            }

            TryBounceToNextEnemy();
            return;
        }

        // Hit wall → destroy
        Destroy(gameObject);
    }

    void TryBounceToNextEnemy()
    {
        currentBounce++;

        if (currentBounce > maxBounces)
        {
            Destroy(gameObject);
            return;
        }

        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();

        GameObject bestTarget = null;
        float bestDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < bestDist && dist <= bounceRange && !hitEnemies.Contains(enemy.gameObject))
            {
                bestTarget = enemy.gameObject;
                bestDist = dist;
            }
        }

        if (bestTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        // Aim at next enemy
        Vector3 dir = (bestTarget.transform.position - transform.position).normalized;
        Launch(dir);
    }
}
