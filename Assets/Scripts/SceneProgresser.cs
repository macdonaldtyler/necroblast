/**************************************************************************
 * Filename: SceneLoader.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script handles scene transitions. When the player enters a trigger
 *     zone, it loads the next scene in the build order. If the current scene
 *     is the last one in the build settings, a message is logged indicating
 *     there are no more levels to load. This is useful for managing level
 *     progression in a game.
 * 
 **************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement; // Required to manage scenes

public class SceneLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Load the next scene by build index
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // Ensure the next scene index is valid
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("No more scenes to load. This is the last level.");
            }
        }
    }
}
