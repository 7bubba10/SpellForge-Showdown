using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CapturePointLogic : MonoBehaviour
{
    public static CapturePointLogic Instance;

    [Header("Capture Point Rotation")]
    public List<GameObject> capturePoints = new List<GameObject>();
    public float initialDelay = 5f;
    public float activeTime = 30f;
    public float downtime = 10f;

    private int lastIndex = -1;
    private GameObject currentPoint;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        DisableAllPoints();
        StartCoroutine(RotateCapturePoints());
    }

    void DisableAllPoints()
    {
        foreach (var p in capturePoints)
            p.SetActive(false);
    }

    IEnumerator RotateCapturePoints()
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
            newIndex = Random.Range(0, capturePoints.Count);
        while (newIndex == lastIndex);

        lastIndex = newIndex;
        currentPoint = capturePoints[newIndex];

        currentPoint.SetActive(true);
        Debug.Log("Activated: " + currentPoint.name);
    }
}