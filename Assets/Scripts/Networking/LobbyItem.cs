using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    public Button joinButton;
    public TMP_Text roomCodeText;
    public TMP_Text playersText;

    private string roomCode;

    public void SetUp(string code, int players)
    {
        roomCode = code;
        roomCodeText.text = roomCode;
        playersText.text = $"Players: {players}";
        joinButton.onClick.AddListener(OnJoinedClick);
    }

    private void OnJoinedClick()
    {
        Debug.Log($"[LobbyItemUI] Join clicked for room {roomCode}");
    }
}