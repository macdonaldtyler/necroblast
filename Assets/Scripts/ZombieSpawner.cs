using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;          // The zombie prefab
    public float spawnInterval = 7f;        // Time between spawns
    public int maxZombies = 10;             // Maximum zombies allowed

    private List<GameObject> activeZombies = new List<GameObject>(); // Track active zombies

    void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if there's room to spawn a new zombie
            if (activeZombies.Count < maxZombies)
            {
                // Spawn the zombie at the ZombieSpawner's position
                GameObject newZombie = Instantiate(zombiePrefab, transform.position, transform.rotation);
                activeZombies.Add(newZombie);

                // Listen for when the zombie is destroyed
                EnemyMovement zombieScript = newZombie.GetComponent<EnemyMovement>();
                if (zombieScript != null)
                {
                    zombieScript.OnZombieDestroyed += RemoveZombieFromList;
                }
            }
        }
    }

    void RemoveZombieFromList(GameObject zombie)
    {
        if (activeZombies.Contains(zombie))
        {
            activeZombies.Remove(zombie);
        }
    }
}