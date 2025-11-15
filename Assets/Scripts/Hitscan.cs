using UnityEngine;

public class HitscanWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int damage = 15;
    public float maxDistance = 200f;

    [Header("Effects")]
    public GameObject impactEffect;

    private Camera ownerCamera;

    void Start()
    {
        ownerCamera = GetComponentInChildren<Camera>();
    }

    public void TryFire()
    {
        RaycastHit hit;

        if (Physics.Raycast(ownerCamera.transform.position, ownerCamera.transform.forward, out hit, maxDistance))
        {
            // Damage if object has health
            var hp = hit.collider.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            // Spawn VFX locally
            if (impactEffect != null)
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}