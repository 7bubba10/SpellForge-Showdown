using System.Collections.Generic;
using UnityEngine;

/// AOE "projectile" that does an instant burst when launched.
/// Use as the projectilePrefab for weapons like Lightning.
public class MagicAOE : MonoBehaviour, IMagicLaunchable
{
    [Header("Shape")]
    [Range(1f, 160f)] public float coneAngle = 35f; // full angle in degrees
    public float range = 8f;
    public bool requireLineOfSight = true;
    public LayerMask hitMask = ~0;        // enemies
    public LayerMask obstacleMask = 0;    // walls/level geo that block LOS
    public bool singleHitPerBurst = true;

    [Header("Lifetime")]
    public float cleanupDelay = 0.05f;    // gives any child VFX a frame to spawn

    [Header("Optional VFX (plays at this transform)")]
    public GameObject burstVFX;

    // Set by weapon via IMagicLaunchable
    private int damage = 25;
    private float dummySpeed;             // ignored, but kept for a common signature
    private GameObject owner;

    private readonly Collider[] overlapBuf = new Collider[64];
    private readonly HashSet<Transform> hitSet = new HashSet<Transform>();

    public void Configure(int dmg, float spd)
    {
        damage = dmg;
        dummySpeed = spd;
    }

    public void Launch(Vector3 velocity, GameObject ownerObj)
    {
        owner = ownerObj;

        // Optional VFX on fire
        if (burstVFX) Instantiate(burstVFX, transform.position, transform.rotation);

        StrikeCone(transform.position, transform.forward);
        Destroy(gameObject, cleanupDelay);
    }

    private void StrikeCone(Vector3 origin, Vector3 forward)
    {
        hitSet.Clear();

        float half = coneAngle * 0.5f;
        float maxSqr = range * range;

        // Broad phase
        int count = Physics.OverlapSphereNonAlloc(origin, range, overlapBuf, hitMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            var col = overlapBuf[i];
            if (!col) continue;

            // Don’t zap the owner
            if (owner && col.transform.IsChildOf(owner.transform)) continue;

            var root = col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform;
            if (singleHitPerBurst && hitSet.Contains(root)) continue;

            Vector3 to = col.bounds.center - origin;
            float sqr = to.sqrMagnitude;
            if (sqr > maxSqr) continue;

            if (Vector3.Angle(forward, to) > half) continue;

            // LOS test (optional)
            if (requireLineOfSight && obstacleMask.value != 0)
            {
                if (Physics.Raycast(origin, to.normalized, out var hit, range, obstacleMask, QueryTriggerInteraction.Ignore))
                {
                    if ((hit.point - origin).sqrMagnitude + 0.0001f < sqr) continue;
                }
            }

            // Apply damage
            bool didHit = false;
            if (root.TryGetComponent<Health>(out var hp)) { hp.TakeDamage(damage); didHit = true; }
            if (root.TryGetComponent<EnemyHealth>(out var eh)) { eh.TakeDamage(damage); didHit = true; }

            if (didHit) hitSet.Add(root);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.9f, 1f, 0.25f);
        var origin = transform.position;
        var fwd    = transform.forward;
        float r = Mathf.Tan(0.5f * coneAngle * Mathf.Deg2Rad) * range;

        int steps = 24;
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps * Mathf.PI * 2f;
            Vector3 circle = (transform.right * Mathf.Cos(t) + transform.up * Mathf.Sin(t)) * r;
            Vector3 p = origin + fwd * range + circle;
            if (i > 0) Gizmos.DrawLine(prev, p);
            Gizmos.DrawLine(origin, p);
            prev = p;
        }
    }
#endif
}
