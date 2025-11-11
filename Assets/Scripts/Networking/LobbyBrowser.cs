using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public class LobbyData
{
    public string roomCode;
    public int players;
}

[System.Serializable]
public class LobbyListResponse
{
    public string status;
    public List<LobbyData> lobbies;
}

public class LobbyBrowser : MonoBehaviour
{
    [Header("UI References")]
    public Button refreshButton;
    public Transform lobbyListParent; // Content under Scroll View
    public GameObject lobbyItemPrefab;
    public TMP_Text statusText;

    [Header("Server Config")]
    public string serverUrl = "http://localhost:3003/api/lobbies/list";

    void Start()
    {
        refreshButton.onClick.AddListener(() => StartCoroutine(LoadLobbies()));
    }

    IEnumerator LoadLobbies()
    {
        statusText.text = "Fetching lobbies...";

        using (UnityWebRequest req = UnityWebRequest.Get(serverUrl))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var json = req.downloadHandler.text;
                Debug.Log($"[LobbyBrowser] Received: {json}");
                try
                {
                    var response = JsonUtility.FromJson<LobbyListResponse>(json);
                    PopulateList(response.lobbies);
                    statusText.text = $"Loaded {response.lobbies.Count} lobbies.";
                }
                catch
                {
                    statusText.text = "Error parsing lobby list.";
                }
            }
            else
            {
                statusText.text = $"Error: {req.error}";
            }
        }
    }

    void PopulateList(List<LobbyData> lobbies)
    {
        // Clear old items
        foreach (Transform child in lobbyListParent)
            Destroy(child.gameObject);

        foreach (var lobby in lobbies)
        {
            var item = Instantiate(lobbyItemPrefab, lobbyListParent);
            item.GetComponentInChildren<TMP_Text>().text = $"Code: {lobby.roomCode} ({lobby.players} players)";
            
            // Optional: hook up join button
            Button joinBtn = item.GetComponentInChildren<Button>();
            joinBtn.onClick.AddListener(() =>
            {
                Debug.Log($"Joining lobby {lobby.roomCode}");
                // Later connect to LobbyManager.JoinLobby(lobby.roomCode)
            });
        }
    }
}
