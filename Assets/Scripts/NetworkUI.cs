using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    public Button HostButton;
    public Button ClientButton;
    public Button QuitButton;

    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();

        HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            if (canvas) canvas.enabled = false; // hide UI when host starts
        });

        ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            if (canvas) canvas.enabled = false; // hide UI when client starts
        });

        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
