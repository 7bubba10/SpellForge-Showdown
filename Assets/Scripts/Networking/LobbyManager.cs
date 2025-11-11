using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

[System.Serializable]
public class RoomCodeData
{
    public string roomCode;
}

[System.Serializable]
public class LobbyResponse
{
    public string status;
    public string roomCode;
}

public class LobbyManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button createLobbyButton;
    public Button joinLobbyButton;
    public TMP_Text statusText;

    [Header("Server Config")]
    public string serverUrl = "http://localhost:3003/api/lobbies"; // update later
    private string lastCreatedRoomCode;

    [Header("Canvases")]
    public Canvas mainMenuCanvas;
    public Canvas lobbyBrowserCanvas;

    public void ShowLobbyBrowser()
    {
        mainMenuCanvas.enabled = false;
        lobbyBrowserCanvas.enabled = true;
    }

    public void ShowMainMenu()
    {
        mainMenuCanvas.enabled = true;
        lobbyBrowserCanvas.enabled = false;
    }


    void Start()
    {
        ShowMainMenu();
        createLobbyButton.onClick.AddListener(() => StartCoroutine(CreateLobby()));
        joinLobbyButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(lastCreatedRoomCode))
            {
                StartCoroutine(JoinLobby(lastCreatedRoomCode));
                ShowLobbyBrowser();
            }   
            else
            {
                ShowLobbyBrowser();
            }
                
        });
        statusText.text = "Ready.";
    }

    IEnumerator CreateLobby()
    {
        statusText.text = "Creating lobby...";

        string jsonBody = "{}"; // empty body for now
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest req = new UnityWebRequest($"{serverUrl}/create", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                statusText.text = $"Lobby created: {json}";

                try
                {
                    var data = JsonUtility.FromJson<LobbyResponse>(json);
                    lastCreatedRoomCode = data.roomCode;
                    Debug.Log($"[LobbyManager] Saved Room Code: {lastCreatedRoomCode}");
                }
                catch
                {
                    Debug.LogWarning("Failed to parse room code from response");
                }
            }
            else
            {
                statusText.text = $"Error: {req.error}";
            }
        }
    }

    IEnumerator JoinLobby(string roomCode)
    {
        statusText.text = $"Joining {roomCode}...";

        RoomCodeData data = new RoomCodeData();
        data.roomCode = roomCode;

        string jsonBody = JsonUtility.ToJson(data);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest req = new UnityWebRequest($"{serverUrl}/join", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                statusText.text = $"Joined: {req.downloadHandler.text}";
            else
                statusText.text = $"Error: {req.error}";
        }
    }
}
