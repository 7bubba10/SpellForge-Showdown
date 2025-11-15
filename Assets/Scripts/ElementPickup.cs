using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ElementPickup : MonoBehaviour
{
    [Header("Element Settings")]
    public ElementType elementType = ElementType.Fire;
    public float holdTime = 3f;  // seconds player must hold E

    [Header("Optional: UI prompt text (not required)")]
    public string promptText = "Hold E to claim element";

    private bool playerInRange = false;
    private PlayerElementManager currentPlayer;
    private float holdProgress = 0f;

    private void Reset()
    {
        // Make sure collider is trigger by default
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        currentPlayer = other.GetComponent<PlayerElementManager>();
        if (currentPlayer == null) return;

        playerInRange = true;
        holdProgress = 0f;

        Debug.Log($"Player entered {elementType} pickup. {promptText}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.GetComponent<PlayerElementManager>() == currentPlayer)
        {
            playerInRange = false;
            currentPlayer = null;
            holdProgress = 0f;

            Debug.Log("Player left pickup area, progress reset.");
        }
    }

    private void Update()
    {
        if (!playerInRange || currentPlayer == null)
            return;

        // If player is holding E, count up
        if (Input.GetKey(KeyCode.E))
        {
            holdProgress += Time.deltaTime;

            // You can hook a UI bar here by using (holdProgress / holdTime)

            if (holdProgress >= holdTime)
            {
                ClaimElement();
            }
        }
        else
        {
            // If they release E early, reset progress
            if (holdProgress > 0f)
            {
                holdProgress = 0f;
                Debug.Log("E released, claiming progress reset.");
            }
        }
    }

    private void ClaimElement()
    {
        if (currentPlayer == null) return;

        Debug.Log($"Element {elementType} claimed after holding E.");

        currentPlayer.EquipElement(elementType);

        // Destroy the pickup once claimed
        Destroy(gameObject);
    }
}
