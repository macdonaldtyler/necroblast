/**************************************************************************
 * Filename: EnemyMovement.cs
 * Author: Amir Tarbiyat
 * Description:
 *     This script controls the behavior and movement of zombie enemies,
 *     including navigation, attacking, taking damage, dying, and sound
 *     effects. The zombies utilize NavMesh for pathfinding, trigger animations
 *     for various states (idle, walking, attacking, dying), and interact with
 *     the player by causing damage. This script also manages health, the
 *     destruction of zombie enemies, and volume control for different sound
 *     effects, allowing customization of walk, attack, and death sound levels.
 * 
 **************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 1f;
    public float minDistance = 1.5f;
    public Transform target;
    public int damage = 1;
    public int health = 10; // Add health for the enemy
    public float attackCooldown = 1f; // Time between attacks

    [Header("Effects")]
    public GameObject bloodEffectPrefab; // Prefab for blood effects
    public int bloodEffectCount = 10;    // Number of blood effects to spawn
    public float bloodEffectSpawnHeight = 1f; // Height above ground to spawn blood effects
    public float bloodEffectSpawnRadius = 1f; // Radius around the enemy for random spawn positions

    [Header("Sound Effects")]
    public AudioClip[] walkSounds; // Array of ambient walk sounds
    public AudioClip[] attackSounds; // Array of attack sounds
    public AudioClip[] damageSounds; // Array of damage sounds
    public AudioClip deathSound;
    public float walkSoundVolume = 0.5f; // Volume for walk sounds
    public float attackSoundVolume = 0.7f; // Volume for attack sounds
    public float damageSoundVolume = 0.6f; // Volume for damage sounds
    public float deathSoundVolume = 1.0f; // Volume for death sound

    private NavMeshAgent agent;
    private Animator animator;
    private CharacterController characterController;
    private AudioSource audioSource;
    private bool isAttacking = false; // To check if the zombie is currently attacking

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // Add an AudioSource component if it doesn't exist
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        animator.Play("Z_Idle");

        // Set the speed and stopping distance for the NavMeshAgent
        agent.speed = speed;
        agent.stoppingDistance = minDistance;

        // If no target is set, assume it is the player
        if (target == null)
        {
            if (GameObject.FindWithTag("Player") != null)
            {
                target = GameObject.FindWithTag("Player").GetComponent<Transform>();
            }
        }

        // Start playing ambient walk sound
        PlayRandomAmbientSound();
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > minDistance)
        {
            agent.SetDestination(target.position);
            animator.Play("Z_Walk_InPlace");
        }
        else if (!isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }

        float movementSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", movementSpeed);
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        PlayRandomDamageSound(); // Play damage sound when the enemy takes damage
        if (health <= 0)
        {
            Die();
        }
    }

    public delegate void ZombieDestroyed(GameObject zombie);
    public event ZombieDestroyed OnZombieDestroyed;

    private void Die()
    {
        StopAmbientSound();
        PlaySound(deathSound, deathSoundVolume);
        SpawnBloodEffects(); // Spawn blood effects upon death
        OnZombieDestroyed?.Invoke(gameObject);
        StartCoroutine(FallAndDespawn());
    }

    private IEnumerator FallAndDespawn()
    {
        yield return new WaitForSeconds(0f);
        Destroy(gameObject);
    }

    private void SpawnBloodEffects()
    {
        if (bloodEffectPrefab == null)
        {
            Debug.LogWarning("Blood effect prefab is not assigned!");
            return;
        }

        for (int i = 0; i < bloodEffectCount; i++)
        {
            // Generate a random position within a circle (360-degree spread)
            Vector2 randomCircle = Random.insideUnitCircle * bloodEffectSpawnRadius;
            Vector3 spawnPosition = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y + bloodEffectSpawnHeight,
                transform.position.z + randomCircle.y
            );

            Instantiate(bloodEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        StopAmbientSound();

        while (Vector3.Distance(transform.position, target.position) <= minDistance)
        {
            Debug.Log("Zombie is attacking the player."); // Log attack
            animator.Play("Z_Attack");
            PlayRandomAttackSound();

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("PlayerHealth component found. Applying damage."); // Log damage application
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on the target.");
            }

            yield return new WaitForSeconds(attackCooldown); // Wait before the next attack
        }

        isAttacking = false;
        ResumeAmbientSound();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("Z_Attack");
            StopAmbientSound();
            PlayRandomAttackSound();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.Stop(); // Stop ambient sound if playing
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip);
            Invoke(nameof(ResumeAmbientSound), clip.length); // Resume ambient sound after the clip finishes
        }
    }

    private void PlayRandomAttackSound()
    {
        if (attackSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, attackSounds.Length);
            PlaySound(attackSounds[randomIndex], attackSoundVolume);
        }
    }

    private void PlayRandomDamageSound()
    {
        if (damageSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, damageSounds.Length);
            PlaySound(damageSounds[randomIndex], damageSoundVolume);
        }
    }

    private void StopAmbientSound()
    {
        if (audioSource.isPlaying && audioSource.loop)
        {
            audioSource.Pause();
        }
    }

    private void ResumeAmbientSound()
    {
        if (walkSounds.Length > 0 && !audioSource.isPlaying)
        {
            PlayRandomAmbientSound();
        }
    }

    private void PlayRandomAmbientSound()
    {
        if (walkSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, walkSounds.Length);
            audioSource.clip = walkSounds[randomIndex];
            audioSource.volume = walkSoundVolume;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
