using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f;

    float nextFireTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (!projectilePrefab || !firePoint) return;

        // spawn slightly in front so we never overlap the player collider
        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;
        GameObject go = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        // find the shooter collider (CharacterController or any Collider on the player root)
        Collider shooterCol =
            GetComponentInParent<CharacterController>()?.GetComponent<Collider>() ??
            GetComponentInParent<Collider>();

        var proj = go.GetComponent<MagicProjectile>();
        if (proj != null)
        {
            Vector3 launchVel = firePoint.forward * proj.speed;
            proj.Launch(launchVel, shooterCol);
        }
    }
}
