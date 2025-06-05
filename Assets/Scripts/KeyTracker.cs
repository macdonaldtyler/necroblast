/**************************************************************************
 * Filename: KeyTracker.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script tracks whether the player has collected a key. When the
 *     player interacts with the key object (via a trigger), the key's status
 *     is updated using a static variable, and the key object is destroyed
 *     after playing a sound effect once.
 * 
 **************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTracker : MonoBehaviour
{
    public static bool hasKey = false; // Static variable to track key status
    public AudioClip keyPickupSound;   // Sound effect to play when key is picked up

    private AudioSource audioSource;

    private void Start()
    {
        // Attach an AudioSource component to the game object and set the audio clip
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = keyPickupSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasKey = true;
            Debug.Log("Key collected!");

            // Play the key pickup sound effect
            if (keyPickupSound != null)
            {
                audioSource.Play();
            }

            // Destroy the key object after the sound effect finishes playing
            // Use a delay to allow the sound effect to finish before destroying the key
            Destroy(gameObject, keyPickupSound.length);
        }
    }
}
