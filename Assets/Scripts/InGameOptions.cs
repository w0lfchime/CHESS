using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameOptions : MonoBehaviour
{
    [Header("UI References")]
    public Button resumeButton;
    public Button menuButton;
    public Button quitButton;
    public GameObject panel;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // Default name, can be changed in inspector

    void Start()
    {
        // Set up button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMainMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Make sure the pause panel is initially hidden
        if (panel != null)
            panel.SetActive(false);
    }

    public void TogglePauseMenu(bool show)
    {
        Debug.Log($"InGameOptions.TogglePauseMenu called with show: {show}");
        
        if (panel != null)
        {
            panel.SetActive(show);
            Debug.Log($"Panel set to active: {show}");
        }
        else
        {
            Debug.LogError("Panel is null! Please assign the pause menu panel in the InGameOptions inspector.");
        }
    }

    public void ResumeGame()
    {
        // Tell the GameManager to resume
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void GoToMainMenu()
    {
        // Reset time scale before changing scenes
        Time.timeScale = 1f;
        
        // Load the main menu scene
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Main menu scene name is not set!");
        }
    }

    public void QuitGame()
    {
        // Reset time scale before quitting
        Time.timeScale = 1f;
        
        // Quit the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
