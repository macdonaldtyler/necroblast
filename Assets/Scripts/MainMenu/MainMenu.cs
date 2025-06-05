using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Reference to the settings panel
    public GameObject settingsPanel;

    // Method to start the game by loading the specified scene
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1); // Load the scene with index 1 asynchronously
    }

    // Method to quit the game application
    public void QuitGame()
    {
        Application.Quit(); // Close the application
    }

    // Method to toggle the settings panel visibility
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf); // Toggle the panel's active state
        }
    }

    // Method to close the settings panel
    public void CloseSettings()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false); // Set the panel's active state to false to close it
        }
    }
}
