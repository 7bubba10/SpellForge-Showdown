using UnityEngine;

public class SimpleCaptureZone : MonoBehaviour
{
    public float pointsPerSecond = 1f;

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
            playerInside = null;
            CancelInvoke(nameof(GivePoints));
        }
    }

    private void GivePoints()
    {
        if (playerInside != null)
            playerInside.AddScore(1);
    }
}
