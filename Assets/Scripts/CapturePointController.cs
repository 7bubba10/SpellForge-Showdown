using UnityEngine;
using System.Collections.Generic;

public class CapturePointController : MonoBehaviour
{
    [Header("Capture Points")]
    public List<GameObject> capturePoints = new List<GameObject>();

    private int lastIndex = -1;
    private GameObject currentPoint;

    // Turn ALL capture points off
    public void DisableAll()
    {
        foreach (var p in capturePoints)
            p.SetActive(false);
    }

    // Select and activate a NEW capture point (used at start of round)
    public void SelectNewCapturePoint()
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

        Debug.Log("Activated capture point: " + currentPoint.name);
    }

    // Called by GameManager before activating a new point
    public void DeactivateCurrentPoint()
    {
        if (currentPoint != null)
        {
            currentPoint.SetActive(false);
            currentPoint = null;
        }
    }

    // Called by GameManager when a new round starts
    public void ActivatePointForNewRound()
    {
        SelectNewCapturePoint();
    }
}