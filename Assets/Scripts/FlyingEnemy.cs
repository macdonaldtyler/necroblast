/**************************************************************************
 * Filename: FlyingEnemy.cs
 * Author: Amir Tarbiyat
 * Description:
 *     This script controls the movement and behavior of flying enemies.
 *     The enemy can orbit around the player at a specified radius, track 
 *     and approach the player aggressively, and attack using projectiles 
 *     when within range. It also manages health, damage handling, and 
 *     a dynamic death sequence with animations, sound effects, and an 
 *     explosion effect upon hitting the ground. The enemy uses NavMesh 
 *     for pathfinding and includes volume controls for different sound 
 *     effects such as taking damage, falling, and exploding.
 * 
 **************************************************************************/



using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class FlyingEnemyMovement : MonoBehaviour
{
    public float orbitRadius = 3.5f;
    public float orbitSpeed = 3f;
    public float heightOffset = 2f;
    public float approachSpeed = 0.5f;
    public Transform target;
    public int damage = 1;
    public int health = 10;

    public float timeBetweenAttacks = 2f;
    public float attackRange = 10f;
    public float sightRange = 15f;
    public GameObject projectile;

    // Audio
    public AudioClip damageSound; // Audio clip to play when taking damage
    public AudioClip fallingSound; // Audio clip to play when falling
    public AudioClip explosionSound; // Audio clip to play when hitting the ground
    public AudioClip attackSound; // Audio clip to play when attacking

    // Explosion Effect
    public GameObject explosionEffectPrefab; // Explosion effect prefab to instantiate when destroyed

    private NavMeshAgent agent;
    private Animator animator;
    private float angle = 0f;

    private bool alreadyAttacked;
    private bool playerInSightRange;
    private bool playerInAttackRange;
    private bool hasPlayedFallingSound = false; // Ensure falling sound only plays once

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.baseOffset = heightOffset;

        animator.Play("Z_Idle");

        // If there's no target set, assume the player
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }

        // Always face the player
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Prevent tilting up or down
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Check for sight and attack range
        playerInSightRange = Vector3.Distance(transform.position, target.position) <= sightRange;
        playerInAttackRange = Vector3.Distance(transform.position, target.position) <= attackRange;

        if (playerInSightRange)
        {
            AggressiveTracking();
            if (playerInAttackRange)
            {
                AttackPlayer();
            }
        }
        else
        {
            // Move closer to the target if outside sight range
            agent.SetDestination(target.position);
            animator.Play("Z_Fly");
        }
    }

    private void AggressiveTracking()
    {
        float currentDistance = Vector3.Distance(transform.position, target.position);

        // If the enemy is too close, move away
        if (currentDistance < orbitRadius - 1f)
        {
            Vector3 moveDirection = (transform.position - target.position).normalized;
            Vector3 newPosition = transform.position + moveDirection * approachSpeed * Time.deltaTime;
            agent.SetDestination(newPosition);
        }
        // If the enemy is too far, move closer
        else if (currentDistance > orbitRadius + 1f)
        {
            Vector3 moveDirection = (target.position - transform.position).normalized;
            Vector3 newPosition = transform.position + moveDirection * approachSpeed * Time.deltaTime;
            agent.SetDestination(newPosition);
        }
        else
        {
            // Maintain position by orbiting
            OrbitPlayer();
        }

        animator.Play("Z_Fly");
    }

    private void OrbitPlayer()
    {
        // Increment the angle for circular motion
        angle += orbitSpeed * Time.deltaTime;

        // Calculate the position in orbit
        float x = target.position.x + orbitRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = target.position.z + orbitRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
        float y = target.position.y + heightOffset;

        // Use NavMeshAgent to move to the orbit position
        Vector3 orbitPosition = new Vector3(x, y, z);
        agent.SetDestination(orbitPosition);
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
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

            // Calculate the direction to the player
            Vector3 directionToPlayer = (target.position - transform.position).normalized;

            // Adjust the projectile spawn position slightly in front of the enemy
            Vector3 spawnPosition = transform.position + transform.forward * 1.5f;

            // Instantiate and launch the projectile
            Rigidbody rb = Instantiate(projectile, spawnPosition, Quaternion.identity).GetComponent<Rigidbody>();
            rb.velocity = directionToPlayer * 32f; // Adjust speed if necessary

            // Reset attack timer
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (health <= 0) return; // Prevent taking damage when already dead

        health -= damageAmount;

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

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Stop movement, attacks, and tracking
        agent.isStopped = true;
        enabled = false; // Disables Update and other behaviors

        // Start the death fall coroutine
        StartCoroutine(HandleDeathFall());
    }

    private IEnumerator HandleDeathFall()
    {
        animator.Play("Z_Death"); // Play death animation

        // Play falling sound if available and hasn't been played yet
        if (fallingSound != null && !hasPlayedFallingSound)
        {
            hasPlayedFallingSound = true;
            GameObject tempAudioSource = new GameObject("TempAudio");
            tempAudioSource.transform.position = transform.position;

            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = fallingSound;
            audioSource.loop = false;
            audioSource.Play();

            Destroy(tempAudioSource, fallingSound.length); // Destroy the temporary GameObject after sound finishes playing
        }

        // Random rotation speeds for each axis to make it more dynamic
        float rotationSpeedX = Random.Range(300f, 600f);
        float rotationSpeedY = Random.Range(300f, 600f);
        float rotationSpeedZ = Random.Range(300f, 600f);

        // Gradually decrease the height offset and apply rotation
        while (agent.baseOffset > 0f)
        {
            agent.baseOffset -= Time.deltaTime * 5f; // Adjust fall speed as needed

            // Rotate the model across all axes
            transform.Rotate(
                rotationSpeedX * Time.deltaTime,
                rotationSpeedY * Time.deltaTime,
                rotationSpeedZ * Time.deltaTime,
                Space.World
            );

            yield return null; // Wait for the next frame
        }

        agent.baseOffset = 0f; // Ensure the offset is exactly 0

        // Play explosion sound and instantiate explosion effect if available
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudio");
            tempAudioSource.transform.position = transform.position;

            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = explosionSound;
            audioSource.Play();

            Destroy(tempAudioSource, explosionSound.length); // Destroy the temporary GameObject after sound finishes playing
        }

        // Wait 2 seconds before despawning
        yield return new WaitForSeconds(2f);
        Destroy(gameObject); // Despawn the enemy
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        animator.Play("Z_Attack");
    //    }
    //}

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
