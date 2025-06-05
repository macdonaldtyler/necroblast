/**************************************************************************
 * Filename: PlayerHealth.cs
 * Author: Amir Tarbiyat
 * Description:
 *     This script manages the player's health, including taking damage,
 *     healing, dying, updating the health display on the screen, and health
 *     regeneration with a delay after taking damage.
 * 
 **************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement; // Import this to use scene management functions
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public TextMesh healthTextMesh; // Reference to the legacy Text Mesh

    public float regenDelay = 5f; // Delay in seconds after taking damage before regeneration starts
    public float regenAmount = 5f; // Amount of health regenerated per second

    private bool isRegenerating = false; // Flag to determine if regeneration should occur

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to max health at the start
        UpdateHealthText(); // Update the text at the start
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        UpdateHealthText(); // Update the text

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StopCoroutine("RegenerateHealth"); // Stop any ongoing regeneration
            isRegenerating = false;
            StartCoroutine(StartRegenerationAfterDelay());
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Clamp health to max value
        Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
        UpdateHealthText(); // Update the text
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        ReloadScene(); // Reloads the current scene when player dies
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHealthText()
    {
        if (healthTextMesh != null)
        {
            healthTextMesh.text = $"{(int)currentHealth}/{(int)maxHealth}";
        }
    }

    private IEnumerator StartRegenerationAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);
        isRegenerating = true;
        StartCoroutine("RegenerateHealth");
    }

    private IEnumerator RegenerateHealth()
    {
        while (isRegenerating && currentHealth < maxHealth)
        {
            currentHealth += regenAmount * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth); // Clamp health to max value
            UpdateHealthText();
            yield return null; // Wait for the next frame
        }
        isRegenerating = false;
    }
}
