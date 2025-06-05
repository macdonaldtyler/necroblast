/**************************************************************************
 * Filename: TurretEnemy.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script controls the behavior of a turret-style enemy, including
 *     detecting the player within sight and attack ranges, rotating towards
 *     the player, and firing projectiles at the player. The turret can take
 *     damage and be destroyed when its health is depleted. It also provides
 *     visual debugging for sight and attack ranges in the editor using Gizmos.
 *     When the turret takes damage, it plays a sound, which can be customized
 *     in the inspector without requiring an AudioSource component to be attached.
 *     Additionally, it plays a destruction sound and an explosion effect when
 *     the turret is destroyed. A sound effect is also played whenever the turret
 *     fires a projectile at the player, enhancing the feedback for attacks.
 **************************************************************************/

using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public Transform player;

    public LayerMask whatIsPlayer;

    public float health;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // Audio
    public AudioClip attackSound; // Audio clip to play when attacking
    public AudioClip damageSound; // Audio clip to play when taking damage
    public AudioClip destructionSound; // Audio clip to play when destroyed

    // Ranges
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float rotationSpeed = 5f; // Speed for rotating towards the player

    // Explosion Effect
    public GameObject explosionEffectPrefab; // Explosion effect prefab to instantiate when destroyed
    private bool isDestroyed = false; // To prevent repeated destruction

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (isDestroyed) return; // Stop all actions if destroyed

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange)
        {
            RotateTowardsPlayer();
        }

        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Prevent the turret from tilting up or down

        // Smoothly rotate towards the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            // Calculate direction to the player for projectile
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position + transform.forward * 1.5f + Vector3.up * 1.0f, Quaternion.identity).GetComponent<Rigidbody>();

            // Apply force directly towards the player's position with some arc adjustment
            Vector3 forceDirection = (player.position - rb.transform.position).normalized;
            rb.AddForce(forceDirection * 32f, ForceMode.Impulse);

            // Play attack sound if available
            if (attackSound != null)
            {
                GameObject tempAudioSource = new GameObject("TempAudio");
                tempAudioSource.transform.position = transform.position;

                AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
                audioSource.clip = attackSound;
                audioSource.Play();

                Destroy(tempAudioSource, attackSound.length); // Destroy the temporary GameObject after sound finishes playing
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Play damage sound if available
        if (damageSound != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudio");
            tempAudioSource.transform.position = transform.position;

            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = damageSound;
            audioSource.Play();

            Destroy(tempAudioSource, damageSound.length); // Destroy the temporary GameObject after sound finishes playing
        }

        if (health <= 0 && !isDestroyed) // Ensure the enemy is only destroyed once
        {
            isDestroyed = true;
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        // Stop attacking and other behaviors
        CancelInvoke();
        alreadyAttacked = true;

        // Play destruction sound and instantiate explosion effect if available
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (destructionSound != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudio");
            tempAudioSource.transform.position = transform.position;

            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = destructionSound;
            audioSource.Play();

            Destroy(tempAudioSource, destructionSound.length); // Destroy the temporary GameObject after sound finishes playing
        }

        // Cull the model (destroy the enemy GameObject)
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
