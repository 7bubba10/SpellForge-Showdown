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

    private bool isCharging = false;

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

        // auto reload
        if (props.currentAmmo <= 0 && !props.isLoading)
        {
            StartCoroutine(StartReload());
            return;
        }

        // ===============================
        // CHARGED SHOT (STEAM)
        // ===============================
        if (props.isChargedShot)
        {
            HandleChargedShot();
            return;
        }

        // ===============================
        // NORMAL WEAPONS
        // ===============================
        bool shouldFire = props.isAutomatic
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (shouldFire && Time.time >= nextFireTime && !props.isLoading)
        {
            nextFireTime = Time.time + (1f / (baseFireRate * elementFireRateMultiplier));
            ShootNormal();
        }
    }

    // ==================================================
    // CHARGED-SHOT LOGIC
    // ==================================================
    private void HandleChargedShot()
    {
        // Start charging
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            props.currentCharge = props.minCharge;
        }

        // Holding = increase charge
        if (Input.GetMouseButton(0) && isCharging)
        {
            props.currentCharge += Time.deltaTime * props.maxCharge;

            props.currentCharge = Mathf.Clamp(props.currentCharge,
                props.minCharge, props.maxCharge);
        }

        // Release = fire
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;

            ShootChargedShot(props.currentCharge);

            props.currentCharge = 0f;
        }
    }

    private void ShootChargedShot(float charge)
    {
        props.currentAmmo--;

        float damage = elementDamage * charge;
        float speed = elementSpeed * Mathf.Lerp(1f, 1.4f, charge / props.maxCharge);

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;

        GameObject projObj = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);
        MagicProjectile proj = projObj.GetComponent<MagicProjectile>();

        proj.damage = Mathf.RoundToInt(damage);
        proj.speed = speed;

        proj.Launch(firePoint.forward * proj.speed, this.transform.root.gameObject);
    }

    // ==================================================
    // NORMAL SHOOT
    // ==================================================
    private void ShootNormal()
    {
        props.currentAmmo--;

        GameObject projObj = Instantiate(projectilePrefab,
            firePoint.position + firePoint.forward * 0.6f,
            firePoint.rotation);

        MagicProjectile proj = projObj.GetComponent<MagicProjectile>();

        proj.speed = elementSpeed;
        proj.damage = elementDamage;

        proj.Launch(firePoint.forward * proj.speed, this.transform.root.gameObject);
    }

    private System.Collections.IEnumerator StartReload()
    {
        props.isLoading = true;
        yield return new WaitForSeconds(props.reloadTime);
        props.Reload();
    }
}