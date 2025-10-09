using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string singleplayerSceneName;
    public string multiplayerSceneName;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playSinglePlayerGame()
    {
        SceneManager.LoadScene(singleplayerSceneName);
    }
    public void playMultiplayerGame()
    {
        SceneManager.LoadScene(multiplayerSceneName);
    }
    
    public void quitGame()
    {
        Application.Quit();
    }
}
