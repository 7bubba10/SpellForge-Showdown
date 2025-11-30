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

    [Header("Launch VFX (use for Air Slash)")]
    [Tooltip("Prefab spawned once at launch (e.g., AirSlash_Projectile).")]
    public GameObject launchVFXPrefab;
    [Tooltip("Parent the VFX to the projectile so it travels with it.")]
    public bool parentLaunchVFX = true;
    [Tooltip("If parented, force particle systems to last as long as the projectile.")]
    public bool vfxMatchProjectileLifetime = true;
    [Tooltip("Align VFX to projectile travel direction.")]
    public bool vfxFaceVelocity = true;
    [Tooltip("Local offset applied after parenting.")]
    public Vector3 vfxLocalOffset = Vector3.zero;
    [Tooltip("Local rotation (Euler) applied after parenting.")]
    public Vector3 vfxLocalEuler = Vector3.zero;
    [Tooltip("Uniform scale multiplier for the VFX root.")]
    public float vfxScale = 1f;

    private Rigidbody rb;
    private Collider col;
    private GameObject owner;
    private Vector3 startPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        col.isTrigger = true;
    }

    public void Launch(Vector3 direction, GameObject ownerObj)
    {
        owner = ownerObj;
        startPos = transform.position;

        // Reset motion
        SetLinearVelocity(Vector3.zero);
        rb.angularVelocity = Vector3.zero;

        // Ignore owner collisions
        foreach (var oc in owner.GetComponentsInChildren<Collider>(true))
            Physics.IgnoreCollision(col, oc, true);

        // Fire
        SetLinearVelocity(direction);

        // Spawn Air Slash (or any launch VFX) one time
        if (launchVFXPrefab != null)
        {
            Quaternion rot = vfxFaceVelocity ? Quaternion.LookRotation(direction) : transform.rotation;
            GameObject vfx = Instantiate(launchVFXPrefab, transform.position, rot);

            if (parentLaunchVFX)
            {
                vfx.transform.SetParent(transform, worldPositionStays: true);
                vfx.transform.localPosition += vfxLocalOffset;
                vfx.transform.localEulerAngles += vfxLocalEuler;
            }

            if (Mathf.Abs(vfxScale - 1f) > 0.001f)
                vfx.transform.localScale *= vfxScale;

            // Ensure: play once, no loop, last until projectile dies
            var systems = vfx.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in systems)
            {
                var main = ps.main;
                main.loop = false;                         // no looping
                main.startDelay = 0f;                      // fire immediately
                if (parentLaunchVFX && vfxMatchProjectileLifetime)
                {
                    // keep visible the whole flight
                    // (use max to avoid shrinking intended lifetimes)
                    float target = Mathf.Max(main.startLifetime.constantMax, lifetime + 0.05f);
#if UNITY_6000_0_OR_NEWER
                    main.startLifetime = new ParticleSystem.MinMaxCurve(target);
#else
                    main.startLifetime = target;
#endif
                    // local space so it rides with projectile
                    main.simulationSpace = ParticleSystemSimulationSpace.Local;
                }

                // Kick it off
                ps.Clear(true);
                ps.Play(true);
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (other.TryGetComponent<Health>(out var hp))
            hp.TakeDamage(damage);

        if (other.TryGetComponent<EnemyHealth>(out var enemyHp))
            enemyHp.TakeDamage(damage);

        Destroy(gameObject);
    }

    // ---------- helpers ----------
    private void SetLinearVelocity(Vector3 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }
}
