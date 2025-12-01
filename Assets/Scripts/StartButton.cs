using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public Button startButton;
    
    void Start()
    {

        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainScene_BackUp (Jon) - Copy");
        });

    }
    
}
