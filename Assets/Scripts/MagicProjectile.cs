using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public GameObject impactEffect;

    private Rigidbody rb;
    private Collider col;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        col.isTrigger = false;
    }

    public void Launch(Vector3 direction, GameObject ownerObj)
    {
        owner = ownerObj;

        // FULL reset of physics state
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // IGNORE ALL COLLIDERS ON OWNER
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>(true);
        foreach (Collider oc in ownerColliders)
        {
            Physics.IgnoreCollision(col, oc, true);
        }

        // Apply velocity
        rb.linearVelocity = direction.normalized * speed;

        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore owner always
        if (other.gameObject == owner) return;

        // Spawn impact
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Deal damage offline
        Health hp;
        if (other.TryGetComponent<Health>(out hp))
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}