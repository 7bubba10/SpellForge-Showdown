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