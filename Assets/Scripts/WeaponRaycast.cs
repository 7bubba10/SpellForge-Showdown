using System.Collections;
using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float baseFireRate = 5f;

    [Header("Optional VFX")]
    public ParticleSystem loopMuzzleVFX;   // assign the flamethrower loop here (Fire only)

    [HideInInspector] public int   elementDamage;
    [HideInInspector] public float elementSpeed;
    [HideInInspector] public float elementFireRateMultiplier = 1f;

    private float nextFireTime;
    private ElementWeaponProperties props;
    private bool isCharging;

    private void Awake()
    {
        props = GetComponent<ElementWeaponProperties>();
    }

    private void OnDisable()
    {
        ToggleLoopVFX(false);
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

        // Charged-shot (Steam)
        if (props.isChargedShot)
        {
            ToggleLoopVFX(false);
            HandleChargedShot();
            return;
        }

        // Normal weapons (Fire/Air/Water/Earth/etc.)
        bool holding = props.isAutomatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        // start/stop flamethrower loop while firing
        ToggleLoopVFX(holding && !props.isLoading);

        if (holding && Time.time >= nextFireTime && !props.isLoading)
        {
            nextFireTime = Time.time + (1f / (baseFireRate * elementFireRateMultiplier));
            ShootNormal();
        }
    }

    // ----------------- Helpers -----------------

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
                loopMuzzleVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void ShootNormal()
    {
        props.currentAmmo--;

        var spawnPos = firePoint.position + firePoint.forward * 0.6f;
        var projObj  = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        // Prefer the generic interface
        var launchable = projObj.GetComponent<IMagicLaunchable>();
        if (launchable != null)
        {
            launchable.Configure(elementDamage, elementSpeed);
            launchable.Launch(firePoint.forward * elementSpeed, this.transform.root.gameObject);
            return;
        }

        // Back-compat with older prefabs
        var proj = projObj.GetComponent<MagicProjectile>();
        if (proj != null)
        {
            proj.damage = elementDamage;
            proj.speed  = elementSpeed;
            proj.Launch(firePoint.forward * proj.speed, this.transform.root.gameObject);
        }
        else
        {
            Debug.LogWarning("Projectile prefab has neither IMagicLaunchable nor MagicProjectile.");
            Destroy(projObj);
        }
    }

    // -------------- Charged Shot (Steam) --------------

    private void HandleChargedShot()
    {
        // Start charging
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            props.currentCharge = props.minCharge;
        }

        // Accumulate while held
        if (Input.GetMouseButton(0) && isCharging)
        {
            props.currentCharge += Time.deltaTime * props.maxCharge;
            props.currentCharge = Mathf.Clamp(props.currentCharge, props.minCharge, props.maxCharge);
        }

        // Release to fire
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

        float dmg   = elementDamage * charge;
        float speed = elementSpeed * Mathf.Lerp(1f, 1.4f, charge / props.maxCharge);

        var spawnPos = firePoint.position + firePoint.forward * 0.6f;
        var projObj  = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        var launchable = projObj.GetComponent<IMagicLaunchable>();
        if (launchable != null)
        {
            launchable.Configure(Mathf.RoundToInt(dmg), speed);
            launchable.Launch(firePoint.forward * speed, this.transform.root.gameObject);
        }
        else
        {
            var proj = projObj.GetComponent<MagicProjectile>();
            if (proj != null)
            {
                proj.damage = Mathf.RoundToInt(dmg);
                proj.speed  = speed;
                proj.Launch(firePoint.forward * proj.speed, this.transform.root.gameObject);
            }
            else
            {
                Debug.LogWarning("ChargedShot prefab missing IMagicLaunchable/MagicProjectile.");
                Destroy(projObj);
            }
        }
    }

    private IEnumerator StartReload()
    {
        props.isLoading = true;
        yield return new WaitForSeconds(props.reloadTime);
        props.Reload();
    }
}
