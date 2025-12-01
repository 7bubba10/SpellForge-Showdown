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

    [Header("VFX (optional)")]
    [Tooltip("Prefab that stays attached while the projectile travels (e.g., a swirling void orb).")]
    public GameObject attachedVFXPrefab;
    public Vector3 vfxLocalOffset;
    public Vector3 vfxLocalEuler;
    public bool destroyVFXOnDeath = true;

    [Tooltip("One-shot VFX spawned at each hit (enemy or wall). Leave empty if you don't want an impact flash.")]
    public GameObject hitVFXPrefab;
    public float hitVFXLifetime = 3f;

    Rigidbody rb;
    int currentBounce = 0;
    readonly HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    GameObject spawnedVFX;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        // Attach travel VFX
        if (attachedVFXPrefab != null && spawnedVFX == null)
        {
            spawnedVFX = Instantiate(attachedVFXPrefab, transform);
            spawnedVFX.transform.localPosition = vfxLocalOffset;
            spawnedVFX.transform.localRotation = Quaternion.Euler(vfxLocalEuler);
            spawnedVFX.transform.localScale = Vector3.one;

            // Make sure particles begin immediately
            foreach (var ps in spawnedVFX.GetComponentsInChildren<ParticleSystem>(true))
            {
                var main = ps.main; main.playOnAwake = true; main.startDelay = 0f;
                ps.Simulate(0f, true, true);
                ps.Play(true);
                // IMPORTANT: for attached effects, set Simulation Space = Local in the prefab
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        // Optional one-shot impact VFX (enemy or wall)
        if (hitVFXPrefab)
        {
            var contact = collision.GetContact(0);
            var rot = Quaternion.LookRotation(contact.normal);
            SpawnOneShot(hitVFXPrefab, contact.point, rot, hitVFXLifetime);
        }

        // Damage enemy and keep chaining
        var enemy = other.GetComponent<EnemyHealth>();
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

        // Hit non-enemy → stop
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

        foreach (var e in enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < bestDist && dist <= bounceRange && !hitEnemies.Contains(e.gameObject))
            {
                bestTarget = e.gameObject;
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

    void SpawnOneShot(GameObject prefab, Vector3 pos, Quaternion rot, float life)
    {
        var go = Instantiate(prefab, pos, rot);
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main; main.loop = false; main.playOnAwake = true; main.startDelay = 0f;
            var em = ps.emission; em.rateOverTime = 0f; // use bursts only
            ps.Play(true);
        }
        Destroy(go, life);
    }

    void OnDestroy()
    {
        if (destroyVFXOnDeath && spawnedVFX)
            Destroy(spawnedVFX);
    }
}
