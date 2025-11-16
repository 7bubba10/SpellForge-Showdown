using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public GameObject impactEffect;
    public float knockbackForce = 6f; // Nhow hard the enemy gets pushed

    private Rigidbody rb;
    private Collider col;
    private GameObject owner;

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

        // Reset physics 
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Ignore owner's colliders
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>(true);
        foreach (Collider oc in ownerColliders)
        {
            Physics.IgnoreCollision(col, oc, true);
        }

        // Shoot
        rb.linearVelocity = direction;

        // Auto destroy
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore owner
        if (other.gameObject == owner) return;

        // Spawn hit VFX
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Hit PLAYER
        if (other.TryGetComponent<Health>(out var playerHp))
        {
            playerHp.TakeDamage(damage);
        }

        // Hit ENEMY (with knockback)
        if (other.TryGetComponent<EnemyHealth>(out var enemyHp))
        {
            Vector3 knockDir = (other.transform.position - owner.transform.position).normalized;
            enemyHp.TakeDamage(damage, knockDir, knockbackForce);
        }

        Destroy(gameObject);
    }
}