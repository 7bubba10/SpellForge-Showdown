using UnityEngine;
using Unity.Netcode;

public class MatchStarter : MonoBehaviour
{
    public void StartMatchAsHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MainScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void StartMatchAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
