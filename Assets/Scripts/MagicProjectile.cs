using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MagicProjectile : MonoBehaviour
{
    public float speed      = 15f;
    public float lifetime   = 3f;
    public int   damage     = 15;
    public float maxDistance = 40f;
    public GameObject impactEffect; // optional hit VFX

    Rigidbody rb;
    Collider  col;
    GameObject owner;
    Vector3 startPos;

    void Awake()
    {
        rb  = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        col.isTrigger = true;
    }

    public void Launch(Vector3 velocity, GameObject ownerObj)
    {
        owner = ownerObj;
        startPos = transform.position;

        rb.linearVelocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        foreach (var oc in owner.GetComponentsInChildren<Collider>(true))
            Physics.IgnoreCollision(col, oc, true);

        rb.linearVelocity = velocity;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (other.TryGetComponent<Health>(out var hp))      hp.TakeDamage(damage);
        if (other.TryGetComponent<EnemyHealth>(out var eh)) eh.TakeDamage(damage);

        Destroy(gameObject);
    }
}
