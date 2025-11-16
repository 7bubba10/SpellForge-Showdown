using UnityEngine;

public class AirSpellCaster : MonoBehaviour
{
    [Header("AOE Prefab")]
    public AOEHitbox airAoePrefab;

    [Header("Spell Settings")]
    public float aoeLifetime = 0.5f;     // burst happens instantly
    public float knockbackForce = 20f;   // passed to the AirBurstAOE script
    public float upwardBoost = 2f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click triggers spell
        {
            CastAirBurst();
        }
    }

    void CastAirBurst()
    {
        Vector3 pos = transform.position;

        // Spawn AOE
        AOEHitbox aoe = AOESpawner.SpawnAOE(
            pos,
            Quaternion.identity,
            airAoePrefab,
            0,          // air spell = no direct damage
            true,       // permeable
            aoeLifetime
        );

        // Configure knockback
        AirBurstAOE burst = aoe.GetComponent<AirBurstAOE>();
        burst.knockbackForce = knockbackForce;
        burst.upwardBoost = upwardBoost;
    }
}
