using UnityEngine;
using System.Collections;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireRate = 5f;

    [Header("Optional VFX")]
    // Assign your vfx_Flamethrower_01 (ParticleSystem) on the Fire weapon only.
    public ParticleSystem loopMuzzleVFX;
    public GameObject chargedImpactVFX;     // <-- assign Steam_ChargedImpact here


    [HideInInspector] public int   elementDamage;
    [HideInInspector] public float elementSpeed;
    [HideInInspector] public float elementFireRateMultiplier = 1f;

    ElementWeaponProperties props;
    float nextFireTime;
    bool  isCharging;

    void Awake()    { props = GetComponent<ElementWeaponProperties>(); }
    void OnEnable() { ToggleLoopVFX(false); }
    void OnDisable(){ ToggleLoopVFX(false); }

    void Update()
    {
        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && !props.isLoading)
        {
            StartCoroutine(StartReload());
            ToggleLoopVFX(false);
            return;
        }

        // Auto reload
        if (props.currentAmmo <= 0 && !props.isLoading)
        {
            StartCoroutine(StartReload());
            ToggleLoopVFX(false);
            return;
        }

        // Charged shot (Steam)
        if (props.isChargedShot)
        {
            ToggleLoopVFX(false); // no loop for charged shot
            HandleChargedShot();
            return;
        }

        // Normal weapons
        bool holding = props.isAutomatic ? Input.GetMouseButton(0)
                                         : Input.GetMouseButtonDown(0);

        // Start/stop flamethrower VFX (only plays on Fire because only that weapon has a reference)
        ToggleLoopVFX(holding && !props.isLoading);

        if (holding && Time.time >= nextFireTime && !props.isLoading)
        {
            nextFireTime = Time.time + (1f / (baseFireRate * elementFireRateMultiplier));
            ShootNormal();
        }
    }

    void ToggleLoopVFX(bool shouldPlay)
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

    void HandleChargedShot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            props.currentCharge = props.minCharge;
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            props.currentCharge += Time.deltaTime * props.maxCharge;
            props.currentCharge = Mathf.Clamp(props.currentCharge, props.minCharge, props.maxCharge);
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            ShootChargedShot(props.currentCharge);
            props.currentCharge = 0f;
        }
    }

    void ShootChargedShot(float charge)
    {
        props.currentAmmo--;

        float dmg = elementDamage * charge;
        float spd = elementSpeed * Mathf.Lerp(1f, 1.4f, charge / props.maxCharge);

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;
        GameObject projObj = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);
        var proj = projObj.GetComponent<MagicProjectile>();

        proj.damage = Mathf.RoundToInt(dmg);
        proj.speed  = spd;
        proj.impactEffect = chargedImpactVFX;
        proj.Launch(firePoint.forward * proj.speed, transform.root.gameObject);
    }

    void ShootNormal()
    {
        props.currentAmmo--;

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;
        GameObject projObj = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);
        var proj = projObj.GetComponent<MagicProjectile>();

        proj.speed  = elementSpeed;
        proj.damage = elementDamage;
        proj.Launch(firePoint.forward * proj.speed, transform.root.gameObject);
    }

    IEnumerator StartReload()
    {
        props.isLoading = true;
        yield return new WaitForSeconds(props.reloadTime);
        props.Reload();
    }
}
