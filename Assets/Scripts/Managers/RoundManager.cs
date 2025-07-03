using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Rounds : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        public int weight;
    }

    public List<EnemyType> enemyTypes; // Assign in Inspector
    public Transform[] spawnPoints;   // Assign in Inspector
    public TextMeshProUGUI roundText; // Assign in Inspector

    public float roundDuration = 60f; // Duration of each round in seconds
    public float spawnRateDelay = 1f; // Time between enemy spawns
    public int baseSpawnCountPerRound = 5; // Base number of enemies to spawn each round
    public int maxEnemiesPerRound = 20; // Maximum number of enemies to spawn in a round
    public int maxConcurrentEnemies = 5; // Maximum number of enemies that can be alive at the same time

    private float roundTimer;
    private float spawnTimer;
    private int currentRound = 0;
    private int enemiesAlive = 0; // Track the number of alive enemies

    public enum RoundState { Combat, Upgrade } // Enum to track the state of the round
    private RoundState currentState = RoundState.Combat; // Current state of the round


    private void Start()
    {
        StartRound();
    }

    private void Update()
    {
        if (currentState != RoundState.Combat)
        {
            // If not in combat state, skip the rest of the update
            return;
        }

        // Update timers
        roundTimer -= Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // Check if the round timer has reached zero to start the next round
        if (roundTimer <= 0f)
        {
            EndRound();
            return;
        }

        // Check if we can spawn more enemies this round
        if (spawnTimer >= spawnRateDelay && enemiesAlive < maxConcurrentEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f; // Reset spawn timer
        }
    }

    public void StartRound()
    {
        // Increment the round count and update the UI
        currentRound++;
        UpdateRoundUI();

        roundTimer = roundDuration; // Reset round timer
        spawnTimer = 0f; // Reset spawn timer

        spawnRateDelay = Mathf.Max(0.5f, spawnRateDelay - 0.1f); // Decrease spawn delay to make the game more challenging

        Debug.Log($"Starting Round {currentRound} - Max Concurrent Enemies: {maxConcurrentEnemies}, Spawn Rate: {spawnRateDelay}s"); //Debug Purposes to see round start in console

        maxConcurrentEnemies = Mathf.Min(20, 5 + currentRound * 2); // Increase max concurrent enemies based on the current round, capping at 20
    }
    void EndRound()
    {
        ClearRemainingEnemies();
        currentState = RoundState.Upgrade; // Switch to upgrade state
        Debug.Log($"Round {currentRound} ended. Proceed to upgrade phase.");

        StartCoroutine(SkipUpgradePhase()); // Skips the upgrade phase coroutine
    }

    private IEnumerator SkipUpgradePhase()
    {
        yield return new WaitForSeconds(5f); // Wait for 1 second before showing the upgrade panel
        ContinueToNextRound(); // Automatically continue to the next round
    }

    public void ContinueToNextRound()
    {
        if (currentState == RoundState.Upgrade)
        {
            currentState = RoundState.Combat; // Switch back to combat state
            StartRound(); // Start the next round
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab = GetRandomEnemy(); // Get a random enemy based on weights
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // Select a random spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity); // Instantiate the enemy at the spawn point

        // Check if the enemy has a Rigidbody2D component and set its velocity to zero
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.RoundsHealthMultiplier(currentRound); // Set the enemy's health based on the current round
        }

        enemiesAlive++; // Increase the count of alive enemies
    }


    GameObject GetRandomEnemy()
    {
        // Calculate the total weight of all enemy types to use for weighted random selection
        int totalWeight = 0;
        foreach (var enemy in enemyTypes)
            totalWeight += enemy.weight;

        // If no enemies are defined, return null to avoid errors
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // Iterate through the enemy types and select one based on the random value
        foreach (var enemy in enemyTypes)
        {
            cumulativeWeight += enemy.weight;
            if (randomValue < cumulativeWeight)
            {
                return enemy.prefab;
            }
        }

        // Add a fallback return statement to handle all code paths.  
        return null;
    }

    public void EnemyDied()
    {
        enemiesAlive--; // Decrease the count of alive enemies
        if (enemiesAlive < 0) enemiesAlive = 0; // Ensure it doesn't go negative
    }


    private void ClearRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy); // Destroy all remaining enemies
        }

        enemiesAlive = 0; // Reset the count of alive enemies
    }

    void UpdateRoundUI()
    {
        if (roundText != null)
        {
            roundText.text = "Round: " + currentRound;
        }
    }
}