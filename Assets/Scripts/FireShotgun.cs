using System.Collections.Generic;
using UnityEngine;

public class FireShotgun : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera used for aiming (usually the player's FPS camera).")]
    public Camera playerCamera;     

    [Header("Firing Settings")]
    [Tooltip("Shots per second.")]
    public float fireRate = 1f;     
    [Tooltip("Farthest distance of the cone.")]
    public float maxRange = 15f;    
    [Tooltip("Distance for maximum damage region.")]
    public float innerRange = 5f;   
    [Tooltip("Distance for medium damage region.")]
    public float midRange = 10f;    
    [Tooltip("Half-angle of the cone in degrees (e.g. 30 = 60° wide).")]
    public float coneAngle = 30f;

    [Header("Damage Settings")]
    public int damageClose = 40;
    public int damageMid = 25;
    public int damageFar = 10;

    [Header("Layers that can be hit")]
    [Tooltip("Which layers should receive damage. For testing, Everything is fine.")]
    public LayerMask hitLayers = ~0;  // default: everything

    private float nextFireTime = 0f;

    private void Reset()
    {
        // Try to auto-find the camera if not set
        if (playerCamera == null)
        {
            playerCamera = GetComponentInParent<Camera>();
        }
    }

    private void Update()
    {
        // Only do anything if this weapon is active (Fire element equipped)
        if (!gameObject.activeInHierarchy)
            return;

        // Left mouse click with fire-rate limit
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            Fire();
        }
    }

    private void Fire()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("FireShotgun: No playerCamera assigned.");
            return;
        }

        Vector3 origin = playerCamera.transform.position;
        Vector3 forward = playerCamera.transform.forward;

        // Get all colliders around the player up to maxRange
        Collider[] colliders = Physics.OverlapSphere(
            origin,
            maxRange,
            hitLayers,
            QueryTriggerInteraction.Ignore
        );

        // To avoid hitting the same target multiple times
        HashSet<Health> damaged = new HashSet<Health>();

        foreach (Collider col in colliders)
        {
            Health hp = col.GetComponentInParent<Health>();
            if (hp == null || damaged.Contains(hp))
                continue;

            // Don't damage the player themselves
            if (hp.gameObject.CompareTag("Player"))
                continue;

            Vector3 toTarget = hp.transform.position - origin;
            float distance = toTarget.magnitude;
            if (distance <= 0.01f)
                continue;

            // Direction to target
            Vector3 dirToTarget = toTarget / distance;

            // Angle between our look direction and target direction
            float angle = Vector3.Angle(forward, dirToTarget);

            // Outside the cone? Skip it.
            if (angle > coneAngle)
                continue;

            // Decide damage based on distance
            int damage = 0;

            if (distance <= innerRange)
                damage = damageClose;
            else if (distance <= midRange)
                damage = damageMid;
            else
                damage = damageFar;

            hp.TakeDamage(damage);
            damaged.Add(hp);
        }

        Debug.Log("Fire shotgun fired.");
    }

    // Optional: visualize the ranges in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Vector3 origin = playerCamera.transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, innerRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, midRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, maxRange);
    }
}
