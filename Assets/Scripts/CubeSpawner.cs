using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public int cubeCount = 3;
    public float spacing = 3f;

    public void Spawn()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 pos = new Vector3(i * spacing, 1f, 5f);
            Instantiate(cubePrefab, pos, Quaternion.identity);
        }
    }
}