using UnityEngine;

public class ElementWeaponProperties : MonoBehaviour
{
    [Header("Weapon Type Settings")]
    public int pellets = 1;                // Shotgun pellets
    public float spread = 0f;              // Spread angle in degrees
    public bool isSniper = false;          // Sniper uses high damage / low spread
    public bool isAutomatic = false;       // Hold to fire
    public float fireRateMultiplier = 1f;  // For fast-firing elements
}