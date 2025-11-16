using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AOEHitbox : MonoBehaviour
{
    [Header("Runtime Values (set by spawner)")]
    public int damage;
    public bool permeable;
    public float lifetime;

    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();

        if (col == null)
            Debug.LogError("AOEHitbox requires a Collider component.");
    }

    void Start()
    {
        // Set trigger based on permeability
        col.isTrigger = permeable;

        // Auto-destroy after lifetime
        if (lifetime > 0)
            Destroy(gameObject, lifetime);
    }

    // Trigger version (permeable)
    private void OnTriggerEnter(Collider other)
    {
        if (!permeable) return;

        TryDamage(other.gameObject);
    }

    // Solid collision version (not permeable)
    private void OnCollisionEnter(Collision collision)
    {
        if (permeable) return;

        TryDamage(collision.gameObject);
    }

    private void TryDamage(GameObject target)
    {
        // Don't hit the AOE creator/weapon itself (optional)
        if (target == gameObject) return;

        Health hp = target.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
