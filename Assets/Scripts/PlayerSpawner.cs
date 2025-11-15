using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private void Start()
    {
        // ONLY spawn a player if none exists
        if (FindObjectOfType<FirstPersonController>() == null)
        {
            int index = Random.Range(0, spawnPoints.Length);
            Transform spawn = spawnPoints[index];

            Instantiate(playerPrefab, spawn.position, spawn.rotation);
        }
    }
}