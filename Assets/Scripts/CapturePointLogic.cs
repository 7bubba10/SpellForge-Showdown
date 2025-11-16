using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Capture Point Settings")]
    public List<GameObject> capturePoints = new List<GameObject>();
    public float initialDelay = 10f;
    public float activeTime = 30f;
    public float downtime = 10f;

    private int lastIndex = -1;
    private GameObject currentPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DisableAllPoints();
        StartCoroutine(CapturePointRotation());
    }

    void DisableAllPoints()
    {
        foreach (var point in capturePoints)
            point.SetActive(false);
    }

    IEnumerator CapturePointRotation()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            ActivateRandomPoint();
            yield return new WaitForSeconds(activeTime);

            if (currentPoint != null)
                currentPoint.SetActive(false);

            yield return new WaitForSeconds(downtime);
        }
    }

    void ActivateRandomPoint()
    {
        if (capturePoints.Count == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, capturePoints.Count);
        }
        while (newIndex == lastIndex);

        lastIndex = newIndex;

        currentPoint = capturePoints[newIndex];
        currentPoint.SetActive(true);

        // Notify players
        Debug.Log($"Capture Point Active: {currentPoint.name}");
        NotificationUI.Instance.ShowMessage($"Capture Point Active: {currentPoint.name}");

    }
}
