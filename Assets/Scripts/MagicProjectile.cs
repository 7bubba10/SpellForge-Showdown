using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : NetworkBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 15;
    public GameObject impactEffect;

    [Header("AOE Options")]
    public bool spawnAOE = false;            // Toggle feature
    public GameObject aoePrefab;             // Prefab reference
    public float aoeRadius = 3f;
    public int aoeDamage = 10;

    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        col.isTrigger = false;
    }

    /// <summary>
    /// Called by WeaponRaycast when projectile is spawned.
    /// </summary>
    public void Launch(Vector3 velocity)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = velocity;  // Unity 6 uses linearVelocity

        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Ignore collisions with owner
    /// </summary>
    public void IgnoreOwnerCollision(GameObject owner)
    {
        Collider[] ownerCols = owner.GetComponentsInChildren<Collider>();

        foreach (Collider oc in ownerCols)
        {
            if (oc != null)
                Physics.IgnoreCollision(col, oc, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        if (other.TryGetComponent<Health>(out var hp))
            hp.DealDamageServerRpc(damage);

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (spawnAOE && aoePrefab != null)
        {
            SpawnAOE(transform.position);
        }
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    private void SpawnAOE(Vector3 pos)
    {
        GameObject aoeObj = Instantiate(aoePrefab, pos, Quaternion.identity);
        NetworkObject aoeNO = aoeObj.GetComponent<NetworkObject>();
        aoeNO.Spawn();

        // Immediately tell the AOE to initialize damage
        if (aoeObj.TryGetComponent(out AOEHitbox aoe))
        {
            aoe.InitializeAOE(aoeRadius, aoeDamage);
        }
    }
}
