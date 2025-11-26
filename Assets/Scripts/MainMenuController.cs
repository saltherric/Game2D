using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject howToPlayPanel;

    // Called when Start Game button is pressed
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Show How To Play panel
    public void ShowHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    // Close panel
    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }
}
