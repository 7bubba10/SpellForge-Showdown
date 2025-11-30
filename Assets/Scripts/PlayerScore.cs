using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [Header("Capture Progress")]
    public int captureProgress = 0;
    public int captureGoal = 100; // default; can be changed per round

    public int score;

    public void AddScore(int amount)
    {
        score += amount;

        ScoreUI.Instance.UpdateScore(score);

        var sm = FindObjectOfType<ScoreManager>();
        if (sm != null)
            sm.score += amount;
    }

    public void AddCaptureProgress(int amount)
    {
        captureProgress += amount;

        if (ScoreUI.Instance != null)
        {
            ScoreUI.Instance.UpdateCaptureBar((float)captureProgress / captureGoal);
            ScoreUI.Instance.UpdateScore(captureProgress);
        }

        if (HasFilledCaptureGoal())
        {
            var gm = GameManager.Instance;
            if (gm != null)
                gm.CaptureComplete();
        }
    }

    public bool HasFilledCaptureGoal()
    {
        return captureProgress >= captureGoal;
    }

    public void ResetCaptureProgress()
    {
        captureProgress = 0;
    }
}
