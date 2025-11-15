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

    [Header("Element Weapons (assign in Inspector)")]
    public GameObject fireWeapon;
    public GameObject earthWeapon;
    public GameObject airWeapon;
    public GameObject waterWeapon;

    [Header("Element Variables")]
    public int fireDamage = 15;
    public int earthDamage = 10;
    public int airDamage = 5;
    public int waterDamage = 20;

    public float fireSpeed = 10;
    public float earthSpeed = 15;
    public float airSpeed = 20;
    public float waterSpeed = 5;

    private void Start()
    {
        EquipElement(currentElement);
    }

    public void EquipElement(ElementType newElement)
    {
        currentElement = newElement;

        // Turn everything off first
        SetAllInactive();

        // Turn on the selected one
        switch (currentElement)
        {
            case ElementType.Fire:
                if (fireWeapon) fireWeapon.SetActive(true);
                break;
            case ElementType.Earth:
                if (earthWeapon) earthWeapon.SetActive(true);
                break;
            case ElementType.Air:
                if (airWeapon) airWeapon.SetActive(true);
                break;
            case ElementType.Water:
                if (waterWeapon) waterWeapon.SetActive(true);
                break;
            case ElementType.None:
            default:
                // No weapon in hand
                break;
        }

        Debug.Log($"Equipped element: {currentElement}");
    }

    private void SetAllInactive()
    {
        if (fireWeapon)  fireWeapon.SetActive(false);
        if (earthWeapon) earthWeapon.SetActive(false);
        if (airWeapon)   airWeapon.SetActive(false);
        if (waterWeapon) waterWeapon.SetActive(false);
    }
}
