using UnityEngine;

public class SimpleCaptureZone : MonoBehaviour
{
    public int pointsPerSecond = 1;

    private PlayerScore playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerScore ps))
        {
            playerInside = ps;
            InvokeRepeating(nameof(GivePoints), 1f, 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerScore ps) && ps == playerInside)
        {
            StopScoring();
        }
    }

    private void OnDisable()
    {
        StopScoring();
    }

    private void StopScoring()
    {
        CancelInvoke(nameof(GivePoints));
        playerInside = null;
    }

    private void GivePoints()
    {
        if (playerInside == null)
        {
            CancelInvoke(nameof(GivePoints));
            return;
        }

        playerInside.AddCaptureProgress(pointsPerSecond);
        playerInside.AddScore(pointsPerSecond);

        if (playerInside.HasFilledCaptureGoal())
        {
            GameManager.Instance.CaptureComplete();
            StopScoring();
            return;
        }
    }
}