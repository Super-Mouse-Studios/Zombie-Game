using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rounds : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign in Inspector
    public Transform spawnPoint;   // Assign in Inspector
    public int enemiesPerRound = 5;
    public float spawnDelay = 1f;

    private int currentRound = 1;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        enemiesAlive = 0; // Reset before spawning new wave
        for (int i = 0; i < enemiesPerRound; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemiesAlive++;
        enemy.GetComponent<Zombie_Following>().OnDeath += OnEnemyDeath; // Assumes your enemy has an OnDeath event
    }

    void OnEnemyDeath()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0)
        {
            NextRound();
        }
    }

    void NextRound()
    {
        currentRound++;
        StartCoroutine(SpawnWave());
    }
}

