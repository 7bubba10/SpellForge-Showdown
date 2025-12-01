using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public float maxDistance = 40f;
    public GameObject impactEffect;

    [Header("Optional attached VFX (sticks to projectile)")]
    public GameObject attachedVFXPrefab;      // e.g., AirSlash_Projectile, WaterBall_Projectile, etc.
    public bool destroyVFXOnHit = true;
    public Vector3 vfxLocalOffset = Vector3.zero;
    public Vector3 vfxLocalEuler = Vector3.zero;
    public Vector3 vfxLocalScale = Vector3.one;
    public bool suppressVFXMotionModules = true; // turns off Velocity/Force over Lifetime on the attached VFX

    private Rigidbody rb;
    private Collider col;
    private GameObject owner;
    private Vector3 startPos;
    private GameObject spawnedVFX;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Safe defaults
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        col.isTrigger = true;
    }

    /// <summary>
    /// Launch with a world-space velocity vector.
    /// </summary>
    public void Launch(Vector3 velocity, GameObject ownerObj)
    {
        owner = ownerObj;
        startPos = transform.position;

        // Reset body
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Ignore collisions with the owner
        var ownerCols = owner.GetComponentsInChildren<Collider>(true);
        foreach (var oc in ownerCols)
            Physics.IgnoreCollision(col, oc, true);

        // Fire
        rb.linearVelocity = velocity;

        // Spawn & pin optional VFX to this projectile
        if (attachedVFXPrefab != null && spawnedVFX == null)
        {
            spawnedVFX = Instantiate(attachedVFXPrefab, transform);
            var t = spawnedVFX.transform;
            t.localPosition = vfxLocalOffset;
            t.localRotation = Quaternion.Euler(vfxLocalEuler);
            t.localScale = vfxLocalScale;

            // Force immediate start & local simulation so it rides the projectile
            var systems = spawnedVFX.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in systems)
            {
                var main = ps.main;
                main.playOnAwake = true;
                main.startDelay = 0f;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;

                if (suppressVFXMotionModules)
                {
                    var vel = ps.velocityOverLifetime; vel.enabled = false;
                    var force = ps.forceOverLifetime;  force.enabled = false;
                }

                ps.Simulate(0f, true, true);
                ps.Play(true);
            }
        }

        // Life timer
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other.gameObject == owner) return;
        hasHit = true;

        SpawnImpactVFX();

        // Apply damage if the target has a health component
        if (other.TryGetComponent(out Health hp)) hp.TakeDamage(damage);
        if (other.TryGetComponent(out EnemyHealth eh)) eh.TakeDamage(damage);

        if (destroyVFXOnHit && spawnedVFX) Destroy(spawnedVFX);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (destroyVFXOnHit && spawnedVFX) Destroy(spawnedVFX);
    }
   

    // --- Helpers -------------------------------------------------------------

    private void SpawnImpactVFX()
    {
        if (!impactEffect) return;

        // Nice orientation: face opposite travel direction if we have velocity
        Quaternion rot = (rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f)
            ? Quaternion.LookRotation(-rb.linearVelocity.normalized)
            : Quaternion.identity;

        GameObject fx = Instantiate(impactEffect, transform.position, rot);

        // Harden the effect to be one-shot even if the prefab loops
        var systems = fx.GetComponentsInChildren<ParticleSystem>(true);
        float longest = 0f;

        foreach (var ps in systems)
        {
            var main = ps.main;
            main.loop = false;
            main.startDelay = 0f;

            var emission = ps.emission;
            emission.rateOverTime = 0f; // bursts only

            ps.Simulate(0f, true, true);
            ps.Play(true);

            // Estimate total life = duration + max startLifetime (conservative)
            float life = main.duration;
            switch (main.startLifetime.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    life += main.startLifetime.constant; break;
                case ParticleSystemCurveMode.TwoConstants:
                    life += Mathf.Max(main.startLifetime.constantMin, main.startLifetime.constantMax); break;
                default:
                    life = Mathf.Max(life, main.duration * 1.5f); break;
            }
            if (life > longest) longest = life;
        }

        Destroy(fx, Mathf.Max(0.1f, longest));
    }
}
