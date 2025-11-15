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
        return;

        // Spawn projectile
        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        MagicProjectile proj = projObj.GetComponent<MagicProjectile>();

        if (proj != null)
        {
            // Get player's element data
            PlayerElementManager elem = firePoint.GetComponentInParent<PlayerElementManager>();

            if (elem != null)
            {
                switch (elem.currentElement)
                {
                    case ElementType.Fire:
                        proj.speed = elem.fireSpeed;
                        proj.damage = (int)elem.fireDamage;
                        break;

                    case ElementType.Earth:
                        proj.speed = elem.earthSpeed;
                        proj.damage = (int)elem.earthDamage;
                        break;

                    case ElementType.Air:
                        proj.speed = elem.airSpeed;
                        proj.damage = (int)elem.airDamage;
                        break;

                    case ElementType.Water:
                        proj.speed = elem.waterSpeed;
                        proj.damage = (int)elem.waterDamage;
                        break;
                }
            }

            proj.Launch(firePoint.forward * proj.speed, gameObject);
        }
    }
}