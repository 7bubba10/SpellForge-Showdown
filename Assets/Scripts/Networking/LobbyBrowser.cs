using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
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
    public Transform lobbyListParent;   // Content under Scroll View
    public GameObject lobbyItemPrefab;  // Prefab showing room info + join button
    public TMP_Text statusText;

    [Header("Server Config")]
    public string serverUrl = "http://localhost:3003/api/lobbies"; // base route only

    void Start()
    {
        // Button calls LoadLobbies when clicked
        refreshButton.onClick.AddListener(() => StartCoroutine(LoadLobbies()));
    }

    IEnumerator LoadLobbies()
    {
        statusText.text = "Fetching lobbies...";

        using (UnityWebRequest req = UnityWebRequest.Get($"{serverUrl}/list"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;
                Debug.Log($"[LobbyBrowser] Received: {json}");

                try
                {
                    // Try to parse JSON
                    LobbyListResponse response = JsonUtility.FromJson<LobbyListResponse>(json);

                    if (response != null && response.lobbies != null)
                    {
                        PopulateList(response.lobbies);
                        statusText.text = $"Loaded {response.lobbies.Count} lobbies.";
                    }
                    else
                    {
                        statusText.text = "No lobbies found.";
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[LobbyBrowser] Parse error: {e.Message}");
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
        // Remove old UI elements
        foreach (Transform child in lobbyListParent)
            Destroy(child.gameObject);

        // Create a new UI entry for each lobby
        foreach (var lobby in lobbies)
        {
            GameObject itemObj = Instantiate(lobbyItemPrefab, lobbyListParent);
            LobbyItem item = itemObj.GetComponent<LobbyItem>();

            if (item != null)
            {
                item.SetUp(lobby.roomCode, lobby.players);
            }
            else
            {
                Debug.LogError("[LobbyBrowser] Missing LobbyItem component on prefab!");
            }
        }
    }
}
