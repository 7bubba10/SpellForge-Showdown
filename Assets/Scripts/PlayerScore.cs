using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score;

    public void AddScore(int amount)
    {
        score += amount;
        ScoreUI.Instance.UpdateScore(score);
    }
}
