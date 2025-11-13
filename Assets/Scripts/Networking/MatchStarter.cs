using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MatchStarter : MonoBehaviour
{
    public void StartMatchAsHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void StartMatchAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
