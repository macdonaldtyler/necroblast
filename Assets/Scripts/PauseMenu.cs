using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject player; // Reference to the player

    [SerializeField] 
    private GameObject pauseMenu; // Reference to the pause menu UI element
    [SerializeField]
    private GameObject settingsPanel; // Reference to the settings panel

    private bool isPaused = false; // Track if the game is paused

    void Start()
    {
        // Initially unlock the cursor and show it
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ensure the pause menu and settings panel are not visible at the start
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false); // Make sure settings panel is hidden
    }

    void Update()
    {
        // Toggle pause menu when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume(); // Resume if already paused
            }
            else
            {
                Pause(); // Pause if not paused
            }
        }
    }

    // Method to pause the game and show the menu
    public void Pause()
    {
        pauseMenu.SetActive(true); // Show the pause menu
        
        // Unlock and show the cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Stop game time (pause the game)
        Time.timeScale = 0;

        isPaused = true; // Set paused state
    }

    // Method to resume the game from the pause menu
    public void Resume()
    {
        pauseMenu.SetActive(false); // Hide the pause menu
        settingsPanel.SetActive(false); // Hide the settings panel if it's visible

        // Lock the cursor and hide it for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resume game time
        Time.timeScale = 1;

        isPaused = false; // Set unpaused state
    }

    // Method to return to the first scene (Main Menu)
    public void GoToMainMenu() 
    {
        Time.timeScale = 1; // Ensure game time is reset
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
        SceneManager.LoadScene(0); // Load the first scene in the build settings
    }

    // Method to restart the current level/scene
    public void RestartLevel()
    {
        Time.timeScale = 1; // Ensure game time is reset
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor for gameplay
        Cursor.visible = false; // Hide the cursor
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    // Method to quit the game application
    public void QuitGame()
    {
        Time.timeScale = 1; // Reset game time (not necessary, but good practice)
        Application.Quit(); // Close the application
        // Note: Application.Quit does nothing in the editor; use it in a built application.
    }

    // Method to open the settings panel
    public void OpenSettings()
    {
        settingsPanel.SetActive(true); // Show the settings panel
        pauseMenu.SetActive(false); // Hide the pause menu when settings are open
    }

    // Method to close the settings panel and return to the pause menu
    public void CloseSettings()
    {
        settingsPanel.SetActive(false); // Hide the settings panel
        pauseMenu.SetActive(true); // Show the pause menu again
    }
}