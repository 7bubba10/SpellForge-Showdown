using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI notificationText;

    [Header("Settings")]
    public float displayTime = 3f;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        notificationText.text = message;
        gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        gameObject.SetActive(false);
    }
}
