using UnityEngine;
using Unity.Netcode;

public class WeaponRaycast : NetworkBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f;

    private float nextFireTime;

    void Update()
    {
        // Only allow the local player to control shooting
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            ShootServerRpc();
        }
    }

    // Run on the server spawns projectile for all players
    [ServerRpc]
    void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position + firePoint.forward * 0.6f,
            firePoint.rotation
        );

        projectile.GetComponent<NetworkObject>().Spawn(); // sync across network

        // Launch it 
        var proj = projectile.GetComponent<MagicProjectile>();
        if (proj != null)
        {
            Vector3 launchVel = firePoint.forward * proj.speed;
            proj.Launch(launchVel);
        }
    }
}
