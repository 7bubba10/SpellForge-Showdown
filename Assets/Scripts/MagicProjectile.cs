using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public float speed = 15f;
    public float lifetime = 3f;
    public float damage = 15f;
    public GameObject impactEffect;

    Rigidbody rb;
    Collider myCol;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCol = GetComponent<Collider>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Using trigger-based hits
        myCol.isTrigger = true;
    }

    void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }

    // Called immediately after Instantiate by the spawner
    public void Launch(Vector3 velocity, Collider shooterColliderToIgnore = null)
    {
        if (shooterColliderToIgnore && myCol)
            Physics.IgnoreCollision(myCol, shooterColliderToIgnore, true);

        rb.linearVelocity = velocity; // units/second (do NOT multiply by deltaTime)
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out var health))
            health.TakeDamage(damage);

        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
