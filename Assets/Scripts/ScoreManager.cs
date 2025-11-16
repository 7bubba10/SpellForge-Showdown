using UnityEngine;
using UnityEngine.SceneManagement;
using Modularify.LoadingBars3D;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int scoreToWin = 100;
    public GameObject victoryScreen;
    public string lobbySceneName = "LobbyScene";

    void Update()
    {
        if (LoadingBarStraight.Instance != null)
        {
            float fillAmount = (float)score / scoreToWin;
            LoadingBarStraight.Instance.SetPercentage(fillAmount);
        }

        if (score >= scoreToWin)
            Victory();
    }

    void Victory()
    {
        // Show victory screen
        victoryScreen.SetActive(true);

        // Freeze game
        Time.timeScale = 0f;

        // Load lobby after a delay
        StartCoroutine(ReturnToLobby());
    }

    System.Collections.IEnumerator ReturnToLobby()
    {
        yield return new WaitForSecondsRealtime(3f); // 3 sec delay even when paused

        // Reset time scale
        Time.timeScale = 1f;

        SceneManager.LoadScene(lobbySceneName);
    }
}
