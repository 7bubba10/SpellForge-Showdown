using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireRate = 5f;

    [HideInInspector] public int elementDamage;
    [HideInInspector] public float elementSpeed;
    [HideInInspector] public float elementFireRateMultiplier = 1f;

    private float nextFireTime;
    private ElementWeaponProperties props;

    private void Awake()
    {
        props = GetComponent<ElementWeaponProperties>();
    }

    private void Update()
    {
        // manual reload
        if (Input.GetKeyDown(KeyCode.R) && !props.isLoading)
        {
            StartCoroutine(StartReload());
            return;
        }

        // automatic reload
        if (props.currentAmmo <= 0 && !props.isLoading)
        {
            StartCoroutine(StartReload());
            return;
        }

        bool shouldFire = props.isAutomatic
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (shouldFire && Time.time >= nextFireTime && !props.isLoading)
        {
            nextFireTime = Time.time + (1f / (baseFireRate * elementFireRateMultiplier));
            Shoot();
        }
    }

    private System.Collections.IEnumerator StartReload()
    {
        props.isLoading = true;
        yield return new WaitForSeconds(props.reloadTime);
        props.Reload();
    }

    private void Shoot()
    {
        props.currentAmmo--;

        int pelletCount = Mathf.Max(1, props.pellets);

        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion rot = firePoint.rotation;

            if (props.spread > 0f && !props.isSniper)
            {
                Vector3 random = Random.insideUnitCircle * props.spread;
                rot = firePoint.rotation * Quaternion.Euler(random.x, random.y, 0);
            }

            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, rot);
            MagicProjectile proj = projObj.GetComponent<MagicProjectile>();

            proj.speed = elementSpeed;
            proj.damage = elementDamage;

            // FIXED: use THIS weapon’s root as owner
            proj.Launch(firePoint.forward * proj.speed, this.transform.root.gameObject);
        }
    }
}