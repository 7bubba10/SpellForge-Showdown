using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public float maxDistance = 40f;  // safer default
    public GameObject impactEffect;

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
}