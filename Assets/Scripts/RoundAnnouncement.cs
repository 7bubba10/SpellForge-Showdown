using UnityEngine;
using TMPro;

public class RoundAnnouncement : MonoBehaviour
{
    public static RoundAnnouncement Instance;

    public TextMeshProUGUI announcementText;

    private void Awake()
    {
        Instance = this;
        announcementText.text = "";
    }

    public void ShowMessage(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(Show(msg));
    }

    private System.Collections.IEnumerator Show(string msg)
    {
        announcementText.text = msg;
        announcementText.alpha = 1f;

        yield return new WaitForSeconds(2f); // visible duration

        // Fade out
        float t = 1f;
        while (t > 0)
        {
            t -= Time.deltaTime * 1.5f;
            announcementText.alpha = t;
            yield return null;
        }

        announcementText.text = "";
    }
}