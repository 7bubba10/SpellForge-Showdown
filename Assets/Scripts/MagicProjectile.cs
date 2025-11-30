using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public float maxDistance = 40f;
    public GameObject impactEffect;

    private Rigidbody rb;
    private Collider col;
    private GameObject owner;
    private Vector3 startPos;

    // NEW: store the travel direction and prevent double-spawn
    private Vector3 launchDir = Vector3.forward;
    private bool impacted = false;

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

        // store normalized dir once (don’t rely on rb properties that vary by Unity version)
        launchDir = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;

        rb.linearVelocity = Vector3.zero;   // keep your original API
        rb.angularVelocity = Vector3.zero;

        // ignore owner collision
        foreach (var oc in owner.GetComponentsInChildren<Collider>(true))
            Physics.IgnoreCollision(col, oc, true);

        // shoot
        rb.linearVelocity = direction;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (impacted) return;
        if (other.gameObject == owner) return;

        impacted = true;

        // spawn one-shot VFX
        if (impactEffect != null)
        {
            var rot = launchDir.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(-launchDir) : Quaternion.identity;
            var fx = Instantiate(impactEffect, transform.position, rot);

            // harden it: force all systems to one-shot, then nuke as fallback
            var pss = fx.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in pss)
            {
                var main = ps.main;        main.loop = false;
                var em   = ps.emission;    em.rateOverTime = 0f;   // bursts only
                ps.Play(true);
            }
            Destroy(fx, 4f); // belt-and-suspenders
        }

        if (other.TryGetComponent<Health>(out var hp)) hp.TakeDamage(damage);
        if (other.TryGetComponent<EnemyHealth>(out var ehp)) ehp.TakeDamage(damage);

        Destroy(gameObject);
    }
}
