using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Earth,
    Air,
    Water
}

public class PlayerElementManager : MonoBehaviour
{
    [Header("Current Element")]
    public ElementType currentElement = ElementType.None;

    [Header("Element Weapons")]
    public GameObject fireWeapon;
    public GameObject earthWeapon;
    public GameObject airWeapon;
    public GameObject waterWeapon;

    [Header("Element Damage")]
    public int fireDamage = 15;
    public int earthDamage = 10;
    public int airDamage = 5;
    public int waterDamage = 20;

    [Header("Element Speed")]
    public float fireSpeed = 10f;
    public float earthSpeed = 15f;
    public float airSpeed = 20f;
    public float waterSpeed = 5f;

    private void Start()
    {
        EquipElement(currentElement);
    }

    public void EquipElement(ElementType newElement)
    {
        currentElement = newElement;

        // Disable all weapons first
        fireWeapon.SetActive(false);
        earthWeapon.SetActive(false);
        airWeapon.SetActive(false);
        waterWeapon.SetActive(false);

        switch (newElement)
        {
            case ElementType.Fire:
                SetupWeapon(fireWeapon, fireDamage, fireSpeed, 6, 6f, false, false, 1f);
                break;

            case ElementType.Earth:
                SetupWeapon(earthWeapon, earthDamage, earthSpeed, 1, 0f, false, false, 1f);
                break;

            case ElementType.Air:
                SetupWeapon(airWeapon, airDamage, airSpeed, 1, 0f, false, true, 2f);
                break;

            case ElementType.Water:
                SetupWeapon(waterWeapon, waterDamage, waterSpeed, 1, 0f, true, false, 1f);
                break;
        }

        Debug.Log($"Equipped element: {currentElement}");
    }

    private void SetupWeapon(GameObject weapon, int dmg, float speed, int pellets, float spread, bool sniper, bool auto, float rateMult)
    {
        weapon.SetActive(true);

        var props = weapon.GetComponent<ElementWeaponProperties>();
        props.pellets = pellets;
        props.spread = spread;
        props.isSniper = sniper;
        props.isAutomatic = auto;
        props.fireRateMultiplier = rateMult;

        var ray = weapon.GetComponent<WeaponRaycast>();
        ray.elementDamage = dmg;
        ray.elementSpeed = speed;
        ray.elementFireRateMultiplier = rateMult;
    }
}