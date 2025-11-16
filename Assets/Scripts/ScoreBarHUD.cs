using UnityEngine;

public class ScoreBarHUD : MonoBehaviour
{
    [Header("Prefab to display in HUD")]
    public GameObject hudPrefab;

    [Header("Position relative to camera")]
    public Vector3 localPosition = new Vector3(0.5f, -0.5f, 2f);
    public Vector3 localRotation = Vector3.zero;
    public Vector3 localScale = Vector3.one * 0.1f; // Small default scale

    private GameObject hudInstance;

    void Start()
    {
        if (hudPrefab == null)
        {
            Debug.LogError("HUD prefab not assigned!");
            return;
        }

        // Instantiate prefab as a child of this camera
        hudInstance = Instantiate(hudPrefab);
        hudInstance.transform.SetParent(transform);

        // Set local position/rotation/scale
        hudInstance.transform.localPosition = localPosition;
        hudInstance.transform.localEulerAngles = localRotation;
        hudInstance.transform.localScale = localScale;
    }
}
