using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player References")]
    public Health playerHealth;
    public PlayerElementManager elementManager;

    [Header("Health UI")]
    public Image healthFillImage;     // Image with 'Filled' type

    [Header("Spell UI")]
    public TMP_Text spellTypeText;        // e.g. "Fire"
    public TMP_Text spellNameText;        // e.g. "Flame Blast"

    [Header("Mana UI")]
    public TMP_Text manaText;             // e.g. "Mana: 5 / 8"
    public int debugCurrentMana = 0;  // for now, set in Inspector
    public int debugMaxMana = 8;

    [Header("Pickup Prompt UI")]
    public GameObject pickupPromptPanel;
    public TMP_Text pickupPromptText;

    private void Start()
    {
        if (pickupPromptPanel != null)
            pickupPromptPanel.SetActive(false);
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateSpellUI();
        UpdateManaUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null || healthFillImage == null) return;

        float frac = 0f;
        if (playerHealth.maxHealth > 0)
        {
            frac = (float)playerHealth.CurrentHealth / playerHealth.maxHealth;
        }

        healthFillImage.fillAmount = Mathf.Clamp01(frac);
    }

    private void UpdateSpellUI()
    {
        if (elementManager == null) return;

        string typeText = "";
        string spellName = "";

        switch (elementManager.currentElement)
        {
            case ElementType.Fire:
                typeText = "Fire";
                spellName = "Flame Blast";
                break;

            case ElementType.Earth:
                typeText = "Earth";
                spellName = "Stone Barrage";
                break;

            case ElementType.Air:
                typeText = "Air";
                spellName = "Gale Shot";
                break;

            case ElementType.Water:
                typeText = "Water";
                spellName = "Tidal Pierce";
                break;

            case ElementType.None:
            default:
                typeText = "No Spell";
                spellName = "";
                break;
            
            case ElementType.Ice:
                typeText = "Ice";
                spellName = "Frost Cannon";
                break;

        }

        if (spellTypeText != null)
            spellTypeText.text = typeText;

        if (spellNameText != null)
            spellNameText.text = spellName;
    }

    private void UpdateManaUI()
    {
        if (manaText == null) return;

        manaText.text = $"Mana: {debugCurrentMana} / {debugMaxMana}";
    }

    public void ShowPickupPrompt(string text)
    {
        if (pickupPromptPanel != null)
            pickupPromptPanel.SetActive(true);

        if (pickupPromptText != null)
            pickupPromptText.text = text;
    }

    public void HidePickupPrompt()
    {
        if (pickupPromptPanel != null)
            pickupPromptPanel.SetActive(false);
    }

    public void SetMana(int current, int max)
    {
        debugCurrentMana = current;
        debugMaxMana = max;
    }
}

