using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    private TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScore(int score)
    {
        text.text = "Score: " + score;
    }
}
