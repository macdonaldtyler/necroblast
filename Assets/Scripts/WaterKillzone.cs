/**************************************************************************
 * Filename: WaterKillZone.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script handles a kill zone triggered when the player falls into
 *     the water. Upon entering the trigger zone, the current scene is 
 *     reloaded, simulating a reset or respawn mechanic. This is useful for 
 *     games with hazards like water that reset player progress.
 * 
 **************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the water
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell into the water!");

            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
