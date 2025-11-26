using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Earth,
    Air,
    Water,
    Steam,
    Ice
}

public class PlayerElementManager : MonoBehaviour
{
    [Header("Two Element Inventory")]
    public ElementType elementA = ElementType.None;
    public ElementType elementB = ElementType.None;
    public bool usingA = true;

    [Header("Current Element")]
    public ElementType currentElement = ElementType.None;

    [Header("Element Weapons (assign in Inspector)")]
    public GameObject fireWeapon;
    public GameObject earthWeapon;
    public GameObject airWeapon;
    public GameObject waterWeapon;
    public GameObject steamWeapon;
    
    public GameObject iceWeapon;

    [Header("Element Damage")]
    public int fireDamage = 15;
    public int earthDamage = 10;
    public int airDamage = 5;
    public int waterDamage = 20;
    public int steamDamage = 25;

    public int iceDamage = 40;

    [Header("Element Speed")]
    public float fireSpeed = 10f;
    public float earthSpeed = 7f;
    public float airSpeed = 20f;
    public float waterSpeed = 12f;
    public float steamSpeed = 12f;

    public float iceSpeed = 30f;

    private void Start()
    {
        EquipElement(currentElement);
    }

    // ===================================================================
    // ADD ELEMENT TO INVENTORY
    // ===================================================================
    public void AddElementToInventory(ElementType newElement)
    {
        if (elementA == newElement || elementB == newElement)
        {
            Debug.Log($"Already have {newElement}, ignoring pickup.");
            return;
        }

        if (elementA == ElementType.None)
        {
            elementA = newElement;
            usingA = true;
            EquipElement(elementA);
            return;
        }

        if (elementB == ElementType.None)
        {
            elementB = newElement;
            usingA = false;
            EquipElement(elementB);
            return;
        }

        Debug.Log("Inventory full!");
    }

    // ===================================================================
    // SWITCH ELEMENT
    // ===================================================================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchElement();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryCraftSteam();
        }
    }

    private void SwitchElement()
    {
        usingA = !usingA;
        currentElement = usingA ? elementA : elementB;

        Debug.Log($"Swapped to element: {currentElement}");
        EquipElement(currentElement);
    }

    // ===================================================================
    // CRAFT (Fire + Water = Steam)
    // ===================================================================
    private void TryCraftSteam()
    {
        if ((elementA == ElementType.Fire && elementB == ElementType.Water) ||
            (elementA == ElementType.Water && elementB == ElementType.Fire))
        {
            elementA = ElementType.Steam;
            elementB = ElementType.None;
            usingA = true;

            Debug.Log("Crafted NEW ELEMENT: STEAM");
            EquipElement(ElementType.Steam);
        }
    }

    // ===================================================================
    // EQUIP WEAPON
    // ===================================================================
    public void EquipElement(ElementType newElement)
    {
        currentElement = newElement;

        // Disable all weapons
        fireWeapon.SetActive(false);
        earthWeapon.SetActive(false);
        airWeapon.SetActive(false);
        waterWeapon.SetActive(false);
        steamWeapon.SetActive(false);
        iceWeapon.SetActive(false);

        GameObject weaponToEnable = null;
        int dmg = 0;
        float speed = 0;
        int pellets = 1;
        float spread = 0;
        bool sniper = false;
        bool auto = false;
        float rateMult = 1f;

        switch (newElement)
        {
            // =============================================================
            // FIRE 
            // =============================================================
            case ElementType.Fire:
                weaponToEnable = fireWeapon;
                dmg = fireDamage;
                speed = 5f;          // slow flame-like speed
                pellets = 1;         // stream
                spread = 0f;         // no spread
                auto = true;         // hold to fire
                sniper = false;
                rateMult = 4f;       // very fast fire rate
                break;

            // =============================================================
            // EARTH 
            // =============================================================
            case ElementType.Earth:
                weaponToEnable = earthWeapon;
                dmg = earthDamage;
                speed = earthSpeed;
                pellets = 1;
                break;

            // =============================================================
            // AIR 
            // =============================================================
            case ElementType.Air:
                weaponToEnable = airWeapon;
                dmg = airDamage;
                speed = airSpeed;
                auto = true;
                rateMult = 2f;
                break;

            // =============================================================
            // WATER 
            // =============================================================
            case ElementType.Water:
                weaponToEnable = waterWeapon;
                dmg = waterDamage;
                speed = waterSpeed;
                sniper = true;
                break;

            // =============================================================
            // ICE  
            // =============================================================
            case ElementType.Ice:
                weaponToEnable = iceWeapon;
                dmg = iceDamage;
                speed = iceSpeed;
                pellets = 1;
                spread = 0f;
                auto = false;      // click-to-fire
                sniper = false;
                rateMult = 0.8f;   // slower than default rate
                break;
            
            // =============================================================
            // STEAM 
            // =============================================================
            case ElementType.Steam:
                weaponToEnable = steamWeapon;
                dmg = steamDamage;
                speed = steamSpeed;
                pellets = 1;        // triple burst
                spread = 0f;        // no shotgun spread
                auto = false;       // click once → burst
                sniper = false;
                rateMult = 1f;
                break;

            

            case ElementType.None:
                Debug.Log("Player has no element equipped.");
                return;
        }

        SetupWeapon(weaponToEnable, dmg, speed, pellets, spread, sniper, auto, rateMult);

        Debug.Log($"Equipped element: {currentElement}");
    }

    // ===================================================================
    // APPLY VALUES TO WEAPON
    // ===================================================================
    private void SetupWeapon(
    GameObject weapon,
    int dmg,
    float speed,
    int pellets,
    float spread,
    bool sniper,
    bool auto,
    float rateMult)
    {
        if (weapon == null) return;

        weapon.SetActive(true);

        ElementWeaponProperties props = weapon.GetComponent<ElementWeaponProperties>();
        WeaponRaycast ray = weapon.GetComponent<WeaponRaycast>();

        // 🔥 Steam = Charged Shot
        if (props != null)
        {
            props.isChargedShot = (currentElement == ElementType.Steam);
            props.currentCharge = 0f;

            // normal stats
            props.pellets = pellets;
            props.spread = spread;
            props.isSniper = sniper;
            props.isAutomatic = auto;
            props.fireRateMultiplier = rateMult;

            props.currentAmmo = props.magazineSize;
            props.isLoading = false;
        }

        if (ray != null)
        {
            ray.elementDamage = dmg;
            ray.elementSpeed = speed;
            ray.elementFireRateMultiplier = rateMult;
        }
    }
}