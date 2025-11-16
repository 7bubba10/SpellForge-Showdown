using UnityEngine;

public class UpdraftPad : MonoBehaviour
{
    [Header("Strength")]
    public float upwardAcceleration = 20f;

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Vector3.up * upwardAcceleration, ForceMode.Acceleration);
        }
    }
}
