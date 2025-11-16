using UnityEngine;

public static class AOESpawner
{
    /// <summary>
    /// Instantiates an AOE hitbox with the desired parameters.
    /// </summary>
    public static AOEHitbox SpawnAOE(
        Vector3 position,
        Quaternion rotation,
        AOEHitbox prefab,
        int damage,
        bool permeable,
        float lifetime)
    {
        if (prefab == null)
        {
            Debug.LogError("AOESpawner: AOE prefab is null.");
            return null;
        }

        AOEHitbox instance =
            Object.Instantiate(prefab, position, rotation);

        instance.damage = damage;
        instance.permeable = permeable;
        instance.lifetime = lifetime;

        return instance;
    }
}
