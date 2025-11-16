using UnityEngine;

public class ElementWeaponProperties : MonoBehaviour
{
    [Header("Weapon Type Settings")]
    public int pellets = 1;                // Shotgun pellets
    public float spread = 0f;              // Spread angle in degrees
    public bool isSniper = false;          // Sniper uses high damage / low spread
    public bool isAutomatic = false;       // Hold to fire
    public float fireRateMultiplier = 1f;  // For fast-firing elements

    [Header("Magazine Settings")]
    public int magazineSize = 8;
    public float reloadTime = 1.5f;

    [HideInInspector] public int currentAmmo;
    [HideInInspector] public bool isReloading;

    private void Start()
    {
        currentAmmo = magazineSize;
        isReloading = false;
    }

    public void Reload()
    {
        currentAmmo = magazineSize;
        isReloading = false;
    }

}