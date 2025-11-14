using UnityEngine;
using Unity.Netcode;

public class WeaponRaycast : NetworkBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f;

    private float nextFireTime;
    private NetworkObject playerNO;

    void Start()
    {
        playerNO = GetComponentInParent<NetworkObject>();
    }

    void Update()
    {
        if (playerNO == null || !playerNO.IsOwner)
            return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("WeaponRaycast: projectilePrefab or firePoint missing!");
            return;
        }

        // Spawn projectile on server
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        NetworkObject projNO = proj.GetComponent<NetworkObject>();
        projNO.SpawnWithOwnership(playerNO.OwnerClientId);

        // Launch projectile
        MagicProjectile mp = proj.GetComponent<MagicProjectile>();
        Vector3 velocity = firePoint.forward * mp.speed;
        mp.Launch(velocity);

        // Ignore owner's colliders
        mp.IgnoreOwnerCollision(playerNO.gameObject);
    }
}