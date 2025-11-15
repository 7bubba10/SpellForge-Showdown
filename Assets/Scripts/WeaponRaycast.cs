using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f;

    private float nextFireTime;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("WeaponRaycast: Missing projectilePrefab or firePoint!");
            return;
        }

        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        MagicProjectile proj = projObj.GetComponent<MagicProjectile>();
        proj.Launch(firePoint.forward, this.gameObject);
    }
}