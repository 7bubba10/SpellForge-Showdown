using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ElementPickup : MonoBehaviour
{
    [Header("Element Settings")]
    public ElementType elementType = ElementType.Fire;
    public float holdTime = 3f; // seconds to hold E

    [Header("Optional: UI prompt text")]
    public string promptText = "Hold E to claim element";

    private bool playerInRange = false;
    private PlayerElementManager currentPlayer;
    private float holdProgress = 0f;

    private void Reset()
    {
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

        // Holding E?
        if (Input.GetKey(KeyCode.E))
        {
            holdProgress += Time.deltaTime;

            if (holdProgress >= holdTime)
            {
                ClaimElement();
            }
        }
        else
        {
            // Released E early
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

        // GIVE ELEMENT INTO PLAYER INVENTORY
        currentPlayer.AddElementToInventory(elementType);

        // Destroy pickup
        Destroy(gameObject);
    }
}