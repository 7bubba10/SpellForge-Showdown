using UnityEngine;
using System.Collections.Generic;

public class CapturePointController : MonoBehaviour
{
    [Header("Capture Points")]
    public List<GameObject> capturePoints = new List<GameObject>();

    private int lastIndex = -1;
    private GameObject currentPoint;

    private void Awake()
    {
        // Ensure no capture point is active by default
        DisableAll();
    }

    // Turn ALL capture points off
    public void DisableAll()
    {
        foreach (var p in capturePoints)
            if (p != null)
                p.SetActive(false);
    }

    // Called by GameManager when starting a new round
    public void ActivatePointForNewRound()
    {
        DeactivateCurrentPoint();   // ensure previous one is off
        SelectNewCapturePoint();    // activate exactly ONE new point
    }

    // Choose a new point that is NOT the same as last round
    private void SelectNewCapturePoint()
    {
        if (capturePoints.Count == 0) return;

        int newIndex;

        // If only 1 point exists, skip randomizing to avoid infinite loop
        if (capturePoints.Count == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, capturePoints.Count);
            }
            while (newIndex == lastIndex);
        }

        lastIndex = newIndex;

        currentPoint = capturePoints[newIndex];
        currentPoint.SetActive(true);

        Debug.Log("Activated capture point: " + currentPoint.name);
    }

    // Called by GameManager when a round ends
    public void DeactivateCurrentPoint()
    {
        if (currentPoint != null)
        {
            currentPoint.SetActive(false);
            currentPoint = null;
        }
    }
}