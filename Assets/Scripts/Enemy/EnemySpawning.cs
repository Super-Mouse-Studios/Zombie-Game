using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public float timeUntilSpawn;
    public GameObject enemyPrefab;
    public float timeBetweenRespawns = 3f; // Time between enemy spawns   
    public float timeVariance = 2f; // Random time variation for respawn time
    public float positionVariance = 10f; // Random position variation for spawn position

    // Update is called once per frame
    void Update()
    {
        if (timeUntilSpawn <= 0)
        {
            Vector3 spawnPosition = transform.position; // Spawner game position
            spawnPosition.y += Random.Range(-positionVariance, positionVariance); // Randomizes the y position of the spawn using positionVariance
            spawnPosition.x += Random.Range(-positionVariance, positionVariance); // Randomizes the x position of the spawn using positionVariance
            Instantiate(enemyPrefab, spawnPosition, transform.rotation); // Spawns the enemy prefab at the calculated position

            timeUntilSpawn = timeBetweenRespawns + Random.Range(0, timeVariance); // Calculates how many seconds between spawns
        }

        timeUntilSpawn -= Time.deltaTime; // Subtracts time since last frame
    }
}
