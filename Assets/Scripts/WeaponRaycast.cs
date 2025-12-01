using UnityEngine;
using System.Collections;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireRate = 5f;

    [Header("Optional VFX")]
    public ParticleSystem loopMuzzleVFX;
    public GameObject chargedImpactVFX;

    [HideInInspector] public int elementDamage;
    [HideInInspector] public float elementSpeed;
    [HideInInspector] public float elementFireRateMultiplier = 1f;

    private ElementWeaponProperties props;
    private float nextFireTime;
    private bool isCharging;

    private void Awake()
    {
        props = GetComponent<ElementWeaponProperties>();
    }

    private void OnEnable()
    {
        ToggleLoopVFX(false);
    }

    private void OnDisable()
    {
        ToggleLoopVFX(false);
    }

    private void Update()
    {
        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && !props.isLoading)
        {
            StartCoroutine(StartReload());
            ToggleLoopVFX(false);
            return;
        }

        // Automatic reload when empty
        if (props.currentAmmo <= 0 && !props.isLoading)
        {
            StartCoroutine(StartReload());
            ToggleLoopVFX(false);
            return;
        }

        // Steam charged-shot
        if (props.isChargedShot)
        {
            ToggleLoopVFX(false);
            HandleChargedShot();
            return;
        }

        // Normal weapons
        bool holding = props.isAutomatic ? Input.GetMouseButton(0)
                                         : Input.GetMouseButtonDown(0);

        ToggleLoopVFX(holding && !props.isLoading);

        if (holding && Time.time >= nextFireTime && !props.isLoading)
        {
            nextFireTime = Time.time + (1f / (baseFireRate * elementFireRateMultiplier));
            ShootNormal();
        }
    }

    // ---------------- VFX MANAGEMENT ----------------

    private void ToggleLoopVFX(bool shouldPlay)
    {
        if (!loopMuzzleVFX) return;

        if (shouldPlay)
        {
            if (!loopMuzzleVFX.isPlaying) loopMuzzleVFX.Play();
        }
        else
        {
            if (loopMuzzleVFX.isPlaying)
                loopMuzzleVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // ---------------- NORMAL SHOT ----------------

    private void ShootNormal()
    {
        props.currentAmmo--;

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;
        GameObject projObj = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        // 1) Your teammate’s IMagicLaunchable system
        var launchable = projObj.GetComponent<IMagicLaunchable>();
        if (launchable != null)
        {
            launchable.Configure(elementDamage, elementSpeed);
            launchable.Launch(firePoint.forward * elementSpeed, transform.root.gameObject);
            return;
        }

        // 2) Backwards compatibility – normal magic bullet
        MagicProjectile mp = projObj.GetComponent<MagicProjectile>();
        if (mp != null)
        {
            mp.damage = elementDamage;
            mp.speed = elementSpeed;

            // Charged VFX only applies to magic projectiles
            mp.impactEffect = chargedImpactVFX;

            mp.Launch(firePoint.forward * elementSpeed, transform.root.gameObject);
            return;
        }

        // 3) Your Void Projectile support (bouncing)
        VoidProjectile vp = projObj.GetComponent<VoidProjectile>();
        if (vp != null)
        {
            vp.damage = elementDamage;
            vp.speed = elementSpeed;
            vp.origin = transform.root.gameObject;

            // Unity 6 safe: Launch(directionOnly)
            vp.Launch(firePoint.forward);

            return;
        }

        Debug.LogWarning($"Projectile '{projObj.name}' has no recognized projectile component!");
        Destroy(projObj);
    }

    // ---------------- CHARGED SHOT (STEAM) ----------------

    private void HandleChargedShot()
    {
        // Start charging
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            props.currentCharge = props.minCharge;
        }

        // While holding
        if (Input.GetMouseButton(0) && isCharging)
        {
            props.currentCharge += Time.deltaTime * props.maxCharge;
            props.currentCharge = Mathf.Clamp(props.currentCharge, props.minCharge, props.maxCharge);
        }

        // Release shot
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

        int chargedDamage = Mathf.RoundToInt(elementDamage * charge);
        float chargedSpeed = elementSpeed *
                             Mathf.Lerp(1f, 1.4f, charge / props.maxCharge);

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;
        GameObject projObj = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        // 1) Interface-based projectile
        var launchable = projObj.GetComponent<IMagicLaunchable>();
        if (launchable != null)
        {
            launchable.Configure(chargedDamage, chargedSpeed);
            launchable.Launch(firePoint.forward * chargedSpeed, transform.root.gameObject);
            return;
        }

        // 2) MagicProjectile charged
        MagicProjectile mp = projObj.GetComponent<MagicProjectile>();
        if (mp != null)
        {
            mp.damage = chargedDamage;
            mp.speed = chargedSpeed;
            mp.impactEffect = chargedImpactVFX;

            mp.Launch(firePoint.forward * chargedSpeed, transform.root.gameObject);
            return;
        }

        // 3) Void projectile should NOT be charged
        VoidProjectile vp = projObj.GetComponent<VoidProjectile>();
        if (vp != null)
        {
            vp.damage = chargedDamage;
            vp.speed = chargedSpeed;
            vp.origin = transform.root.gameObject;

            vp.Launch(firePoint.forward);
            return;
        }

        Debug.LogWarning("ChargedShot prefab missing recognized projectile type.");
        Destroy(projObj);
    }

    // ---------------- RELOAD ----------------

    private IEnumerator StartReload()
    {
        props.isLoading = true;
        yield return new WaitForSeconds(props.reloadTime);
        props.Reload();
    }
}