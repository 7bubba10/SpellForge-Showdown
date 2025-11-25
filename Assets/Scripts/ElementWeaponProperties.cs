using UnityEngine;

public class ElementWeaponProperties : MonoBehaviour
{
    [Header("Weapon Type Settings")]
    public int pellets = 1;
    public float spread = 0f;
    public bool isSniper = false;
    public bool isAutomatic = false;
    public float fireRateMultiplier = 1f;

    [Header("Magazine Settings")]
    public int magazineSize = 8;
    public float reloadTime = 1.5f;

    [HideInInspector] public int currentAmmo;
    [HideInInspector] public bool isLoading = false;

    [Header("CHARGED SHOT (Steam Only)")]
    public bool isChargedShot = false;
    public float minCharge = 0.5f;
    public float maxCharge = 2.0f;
    public float currentCharge = 0f;
    public float chargeSpeed = 1.5f;

    private void Awake()
    {
        currentAmmo = magazineSize;
    }

    public void Reload()
    {
        currentAmmo = magazineSize;
        isLoading = false;
    }
}