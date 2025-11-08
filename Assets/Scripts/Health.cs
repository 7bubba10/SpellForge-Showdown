using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    public float maxHP = 50f;

    private NetworkVariable<float> hp = new NetworkVariable<float>();

    void OnEnable()
    {
        if (IsServer)
            hp.Value = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (!IsServer) return; // only server applies damage

        hp.Value -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP left: {hp.Value}");

        if (hp.Value <= 0)
        {
            Debug.Log($"{gameObject.name} destroyed!");
            GetComponent<NetworkObject>().Despawn(); // sync despawn across clients
        }
    }
}
