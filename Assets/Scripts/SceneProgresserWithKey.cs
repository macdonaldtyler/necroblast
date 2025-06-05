/**************************************************************************
 * Filename: SceneLoaderWithKey.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script handles scene transitions that require a key. When the
 *     player enters a trigger zone, it checks if the key has been collected
 *     (using the static variable from `KeyTracker`). If the key is present,
 *     the next scene in the build order is loaded. If the current scene is
 *     the last one, the game loops back to the first scene. If the key has
 *     not been collected, a message is displayed to the player.
 * 
 **************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderWithKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the key has been collected
            if (KeyTracker.hasKey)
            {
                // Load the next scene
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    // Loop back to the first scene
                    SceneManager.LoadScene(0);
                }
            }
            else
            {
                Debug.Log("You need the key to proceed!");
            }
        }
    }
}


