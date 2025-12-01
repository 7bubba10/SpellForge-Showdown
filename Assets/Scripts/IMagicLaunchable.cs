using UnityEngine;

public interface IMagicLaunchable
{
    // Called by the weapon right before Launch so the projectile knows its stats.
    void Configure(int damage, float speed);

    // velocity = world-space launch vector (speed already applied)
    // owner    = root GameObject that fired (to ignore self hits, etc.)
    void Launch(Vector3 velocity, GameObject owner);
}
