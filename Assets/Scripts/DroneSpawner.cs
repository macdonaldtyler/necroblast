/**************************************************************************
 * Filename: DroneSpawner.cs
 * Author: Amir Tarbiyat B00882695
 * Description:
 *     This script controls the spawning of flying drones in the game.
 *     It allows for managing the number of active drones in the scene,
 *     ensuring it does not exceed a specified limit. The script spawns
 *     drones at regular intervals, tracks active drones, and cleans up
 *     destroyed ones automatically. Drones are instantiated at a defined
 *     spawn point and can be directed to target the player.
 * 
 **************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject dronePrefab;  // Prefab of the drone to spawn
    public Transform spawnPoint;   // Location where the drone will spawn
    public int maxDrones = 5;      // Maximum number of drones at any time
    public float spawnInterval = 5f; // Interval in seconds between spawns

    private List<GameObject> activeDrones = new List<GameObject>(); // Tracks active drones
    private float spawnTimer; // Timer for controlling the spawn interval

    void Update()
    {
        // Update the spawn timer
        spawnTimer += Time.deltaTime;

        // Spawn new drone if the timer exceeds interval and the active drone count is below max
        if (spawnTimer >= spawnInterval && activeDrones.Count < maxDrones)
        {
            SpawnDrone();
            spawnTimer = 0f; // Reset the spawn timer
        }

        // Clean up destroyed drones from the list
        activeDrones.RemoveAll(drone => drone == null);
    }

    private void SpawnDrone()
    {
        if (dronePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Drone prefab or spawn point not set in DroneSpawner.");
            return;
        }

        // Instantiate a new drone at the spawn point
        GameObject newDrone = Instantiate(dronePrefab, spawnPoint.position, spawnPoint.rotation);

        // Add it to the list of active drones
        activeDrones.Add(newDrone);

        // Optionally, set the target for the FlyingEnemyMovement script
        FlyingEnemyMovement droneMovement = newDrone.GetComponent<FlyingEnemyMovement>();
        if (droneMovement != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                droneMovement.SetTarget(player.transform);
            }
        }
    }
}
