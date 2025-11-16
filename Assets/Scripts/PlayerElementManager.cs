using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Earth,
    Air,
    Water,
    Steam    // combined element
}

public class PlayerElementManager : MonoBehaviour
{
    [Header("Two Element Inventory")]
    public ElementType elementA = ElementType.None;
    public ElementType elementB = ElementType.None;
    public bool usingA = true;   // tracks which element is active

    [Header("Current Element (auto managed)")]
    public ElementType currentElement = ElementType.None;

    [Header("Element Weapons (assign in Inspector)")]
    public GameObject fireWeapon;
    public GameObject earthWeapon;
    public GameObject airWeapon;
    public GameObject waterWeapon;
    public GameObject steamWeapon;

    [Header("Element Damage")]
    public int fireDamage = 15;
    public int earthDamage = 10;
    public int airDamage = 5;
    public int waterDamage = 20;
    public int steamDamage = 25;

    [Header("Element Speed")]
    public float fireSpeed = 10f;
    public float earthSpeed = 7f;
    public float airSpeed = 20f;
    public float waterSpeed = 5f;
    public float steamSpeed = 12f;

    private void Start()
    {
        EquipElement(currentElement);
    }

    // ============================================================
    //  ADD ELEMENT INTO A/B INVENTORY (CALLED FROM PICKUP SCRIPT)
    // ============================================================
    public void AddElementToInventory(ElementType newElement)
    {
        // Already have this element
        if (elementA == newElement || elementB == newElement)
        {
            Debug.Log($"Already have {newElement}, ignoring pickup.");
            return;
        }

        // Fill Element A first
        if (elementA == ElementType.None)
        {
            elementA = newElement;
            usingA = true;
            EquipElement(elementA);
            return;
        }

        // Fill Element B second
        if (elementB == ElementType.None)
        {
            elementB = newElement;
            usingA = false;
            EquipElement(elementB);
            return;
        }

        Debug.Log("Inventory full — cannot add more elements.");
    }

    // ============================
    //        SWITCH ELEMENTS
    // ============================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchElement();
        }

        if(Input.GetKeyDown(KeyCode.F)) TryCombineElements();
    }

    private void SwitchElement()
    {
        usingA = !usingA;
        currentElement = usingA ? elementA : elementB;

        Debug.Log($"Swapped to element: {currentElement}");

        EquipElement(currentElement);
    }

    // =========================================================
    //                 COMBINE ELEMENTS (A + B)
    // =========================================================

    private void TryCombineElements()
    {
        // Must have two elements
        if (elementA == ElementType.None || elementB == ElementType.None)
        {
            Debug.Log("Cannot combine — need 2 elements.");
            return;
        }

        ElementType result = ElementType.None;

        // ---- Fire + Water = Steam ----
        if ((elementA == ElementType.Fire && elementB == ElementType.Water) ||
            (elementA == ElementType.Water && elementB == ElementType.Fire))
        {   
            result = ElementType.Steam;
        }

        // Add more combos later here...
        // Example:
        // if ((elementA == ElementType.Air && elementB == ElementType.Earth) || ...)

        if (result == ElementType.None)
        {
            Debug.Log("These two elements cannot be combined.");
            return;
        }

        // Perform combination
        Debug.Log($"Combined {elementA} + {elementB} into {result}!");

        // Store new element in slot A
        elementA = result;

        // Clear slot B
        elementB = ElementType.None;

        // Switch to new combined weapon
        usingA = true;
        EquipElement(result);
    }

    // ============================
    //        EQUIP WEAPON
    // ============================
    public void EquipElement(ElementType newElement)
    {
        currentElement = newElement;

        // Disable all first
        fireWeapon.SetActive(false);
        earthWeapon.SetActive(false);
        airWeapon.SetActive(false);
        waterWeapon.SetActive(false);
        steamWeapon.SetActive(false);

        GameObject weaponToEnable = null;
        int dmg = 0;
        float speed = 0f;
        int pellets = 1;
        float spread = 0;
        bool sniper = false;
        bool auto = false;
        float rateMult = 1f;

        switch (newElement)
        {
            case ElementType.Fire:
                weaponToEnable = fireWeapon;
                dmg = fireDamage;
                speed = fireSpeed;
                pellets = 6;
                spread = 6f;
                auto = false;
                sniper = false;
                break;

            case ElementType.Earth:
                weaponToEnable = earthWeapon;
                dmg = earthDamage;
                speed = earthSpeed;
                pellets = 1;
                break;

            case ElementType.Air:
                weaponToEnable = airWeapon;
                dmg = airDamage;
                speed = airSpeed;
                auto = true;
                rateMult = 2f;
                break;

            case ElementType.Water:
                weaponToEnable = waterWeapon;
                dmg = waterDamage;
                speed = waterSpeed;
                sniper = true;
                break;

            case ElementType.Steam:
                weaponToEnable = steamWeapon;
                dmg = steamDamage;
                speed = steamSpeed;
                auto = true;
                rateMult = 1.3f;
                pellets = 3;
                spread = 4f;
                break;

            case ElementType.None:
                Debug.Log("Player has no element equipped.");
                return;
        }

        SetupWeapon(weaponToEnable, dmg, speed, pellets, spread, sniper, auto, rateMult);

        Debug.Log($"Equipped element: {currentElement}");
    }

    // Weapon Setup Helper
    private void SetupWeapon(GameObject weapon, int dmg, float speed, int pellets, float spread, bool sniper, bool auto, float rateMult)
    {
        if (weapon == null) return;

        weapon.SetActive(true);

        var props = weapon.GetComponent<ElementWeaponProperties>();
        if (props != null)
        {
            props.pellets = pellets;
            props.spread = spread;
            props.isSniper = sniper;
            props.isAutomatic = auto;
            props.fireRateMultiplier = rateMult;

            props.currentAmmo = props.magazineSize;
            props.isReloading = false;
        }

        var ray = weapon.GetComponent<WeaponRaycast>();
        if (ray != null)
        {
            ray.elementDamage = dmg;
            ray.elementSpeed = speed;
            ray.elementFireRateMultiplier = rateMult;
        }
    }
}