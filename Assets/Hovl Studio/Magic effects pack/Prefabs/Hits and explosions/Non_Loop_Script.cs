using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyWhenParticlesDone : MonoBehaviour
{
    ParticleSystem ps;

    void Awake() => ps = GetComponent<ParticleSystem>();

    IEnumerator Start()
    {
        // detach so parent movement can’t retrigger anything
        transform.SetParent(null, true);

        // just in case the prefab is saved stopped
        if (!ps.isPlaying) ps.Play();

        // wait until all child particle systems are done too
        yield return new WaitWhile(() => ps.IsAlive(true));

        Destroy(gameObject);
    }
}

