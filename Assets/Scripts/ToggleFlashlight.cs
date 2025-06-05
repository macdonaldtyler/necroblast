/**************************************************************************
 * Filename: FlashlightToggle.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script allows the player to toggle a flashlight on and off using
 *     the "F" key. The flashlight is enabled by default when the game starts.
 *     This functionality is useful for gameplay elements such as exploration
 *     in dark environments.
 * 
 **************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight; // Reference to the flashlight

    void Start()
    {
        // Ensure the flashlight is on by default
        if (flashlight != null)
        {
            flashlight.enabled = true;
        }
    }

    void Update()
    {
      
        // Toggle flashlight with the "F" key
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }
}