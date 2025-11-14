using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;

    // Networked HP value synchronized across clients
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private bool isDead = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnDestroy()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
        base.OnDestroy();
    }


    // Called by clients to request damage
    [ServerRpc]
    public void DealDamageServerRpc(int amount)
    {
        if (isDead) return;

        currentHealth.Value -= amount;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            Die();
        }
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        
        Debug.Log($"{gameObject.name} HP changed: {previousValue} → {newValue}");
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        // Disable movement/camera for this player
        var controller = GetComponent<FirstPersonController>();
        if (controller) controller.enabled = false;

        var cam = GetComponentInChildren<Camera>();
        if (cam) cam.enabled = false;

        
    }
}
