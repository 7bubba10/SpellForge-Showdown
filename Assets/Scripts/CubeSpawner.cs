using UnityEngine;
using Unity.Netcode;

public class CubeSpawner : NetworkBehaviour
{
    public GameObject cubePrefab;
    public int cubeCount = 3;
    public float spacing = 3f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // only the server spawns cubes

        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 pos = new Vector3(i * spacing, 1f, 5f);
            var cube = Instantiate(cubePrefab, pos, Quaternion.identity);
            cube.GetComponent<NetworkObject>().Spawn(); // broadcast spawn
        }
    }
}
