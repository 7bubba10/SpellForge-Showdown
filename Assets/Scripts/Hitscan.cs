using UnityEngine;
using Unity.Netcode;

public class HitscanWeapon : NetworkBehaviour
{
    [Header("Weapon Settings")]
    public int damage = 15;
    public float maxDistance = 200f;

    [Header("Effects")]
    public GameObject impactEffect;

    private Camera ownerCamera;

    public override void OnNetworkSpawn()
    {
        // Get the player's camera ONLY for the local owner
        if (IsOwner)
        {
            ownerCamera = GetComponentInChildren<Camera>();
        }
    }

    /// <summary>
    /// Called by player input when the weapon is fired.
    /// This runs on the owning client and sends a request to the server.
    /// </summary>
    public void TryFire()
    {
        if (!IsOwner)
            return;

        FireServerRpc(ownerCamera.transform.position, ownerCamera.transform.forward);
    }

    /// <summary>
    /// Server does the actual raycast to prevent cheating.
    /// </summary>
    [ServerRpc]
    private void FireServerRpc(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            // Deal damage if the hit object has a Health component
            if (hit.collider.TryGetComponent<Health>(out var hp))
            {
                hp.DealDamageServerRpc(damage);
            }

            // Spawn impact VFX across clients
            if (impactEffect != null)
                SpawnImpactClientRpc(hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    /// <summary>
    /// Clients spawn the impact visual effect.
    /// </summary>
    [ClientRpc]
    private void SpawnImpactClientRpc(Vector3 pos, Quaternion rot)
    {
        if (impactEffect != null)
            Instantiate(impactEffect, pos, rot);
    }
}
