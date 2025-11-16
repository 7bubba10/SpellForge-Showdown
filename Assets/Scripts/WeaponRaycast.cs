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

    void Update()
    {
        bool shouldFire = props.isAutomatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (shouldFire && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / (baseFireRate * elementFireRateMultiplier));
            Shoot();
        }
    }

    void Shoot()
    {
        int pelletCount = Mathf.Max(1, props.pellets);

        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion spreadRot = firePoint.rotation;

            if (props.spread > 0)
            {
                Vector3 randomSpread = Random.insideUnitCircle * props.spread;
                spreadRot = Quaternion.Euler(
                    firePoint.rotation.eulerAngles + new Vector3(randomSpread.x, randomSpread.y, 0)
                );
            }

            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, spreadRot);
            MagicProjectile proj = projObj.GetComponent<MagicProjectile>();

            proj.speed = elementSpeed;
            proj.damage = elementDamage;
            proj.Launch(firePoint.forward * proj.speed, gameObject);
        }
    }
}