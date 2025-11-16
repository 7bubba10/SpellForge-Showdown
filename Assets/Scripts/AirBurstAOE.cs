using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AirBurstAOE : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 20f;
    public float upwardBoost = 2f;

    void OnTriggerEnter(Collider other)
    {
        // Only knock back objects with rigidbody
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // Direction FROM center → TO object
        Vector3 dir = (other.transform.position - transform.position).normalized;
        dir.y += upwardBoost * 0.1f; // small lift

        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }
}
