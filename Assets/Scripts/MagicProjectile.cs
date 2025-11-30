using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    [Header("Motion & Damage")]
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public float maxDistance = 40f;  // safer default

    [Header("VFX (Impact)")]
    public GameObject impactEffect;  // optional: spawned on hit

    [Header("VFX (While Moving)")]
    public GameObject moveEffectPrefab;          // assign Hovl slash here (only on Air projectile prefab)
    public Vector3 moveEffectLocalOffset = Vector3.zero;
    public Vector3 moveEffectLocalEuler = Vector3.zero;
    public bool faceVelocity = true;             // rotate projectile/VFX to flight direction
    public bool destroyMoveEffectWithProjectile = true;

    private Rigidbody rb;
    private Collider col;
    private GameObject owner;
    private Vector3 startPos;
    private GameObject moveEffectInstance;

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

        // Correct start position
        startPos = transform.position;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Ignore owner collisions
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>(true);
        foreach (Collider oc in ownerColliders)
            Physics.IgnoreCollision(col, oc, true);

        // Shoot
        rb.linearVelocity = direction;

        // Spawn the moving VFX as a child so it follows cleanly
        if (moveEffectPrefab != null)
        {
            moveEffectInstance = Instantiate(moveEffectPrefab, transform);
            moveEffectInstance.transform.localPosition = moveEffectLocalOffset;
            moveEffectInstance.transform.localEulerAngles = moveEffectLocalEuler;
            // Tip: on the slash prefab, set ParticleSystem -> Main -> Simulation Space = Local
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Keep the projectile (and child VFX) facing its flight direction
        if (faceVelocity)
        {
            Vector3 v = rb.linearVelocity;
            if (v.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(v.normalized, Vector3.up);
        }

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

    private void OnDestroy()
    {
        if (!destroyMoveEffectWithProjectile && moveEffectInstance != null)
        {
            // Let the effect finish: detach and let its ParticleSystem Stop Action handle cleanup
            moveEffectInstance.transform.SetParent(null, true);
        }
    }
}
