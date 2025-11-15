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
        // Make sure element starts correctly
        EquipElement(currentElement);
    }

    public void EquipElement(ElementType newElement)
    {
        currentElement = newElement;

        // Disable all weapons
        fireWeapon.SetActive(false);
        earthWeapon.SetActive(false);
        airWeapon.SetActive(false);
        waterWeapon.SetActive(false);

        // Enable the chosen one
        switch (currentElement)
        {
            case ElementType.Fire: fireWeapon.SetActive(true); break;
            case ElementType.Earth: earthWeapon.SetActive(true); break;
            case ElementType.Air: airWeapon.SetActive(true); break;
            case ElementType.Water: waterWeapon.SetActive(true); break;
        }

        Debug.Log($"Equipped element: {currentElement}");
    }
}
