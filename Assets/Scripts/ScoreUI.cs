using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    private TextMeshProUGUI text;
    public UnityEngine.UI.Image captureBar;

    private void Awake()
    {
        Instance = this;
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScore(int score)
    {
        text.text = "Score: " + score;
    }

    public void UpdateCaptureBar(float percent)
    {
        if (captureBar != null)
            captureBar.fillAmount = percent;
    }
}
