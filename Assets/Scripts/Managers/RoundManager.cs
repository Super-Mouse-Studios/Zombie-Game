using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rounds : MonoBehaviour
{

    [SerializeField] private GameObject doctorPrefab;
    [SerializeField] private Transform doctorSpawnPoint; // Assign in Inspector
    private int nextDoctorRound = 3;
    private bool doctorKilled = false;
    private DoctorNpc currentDoctor;
    [SerializeField] private ParticleSystem rainParticleSystem;
    [SerializeField] private Transform[] hordeSpawnPoints;



    [System.Serializable]

    public class EnemyType
    {
        public GameObject prefab;
        public int weight;
        public int hordeAmount;
    }

    public List<EnemyType> enemyTypes; // Assign in Inspector  
    public Transform[] spawnPoints;   // Assign in Inspector  
    public TextMeshProUGUI roundText; // Assign in Inspector  

    [Header("Round Settings")]
    public float roundDuration = 60f;
    public float spawnRateDelay = 1f;
    public int baseSpawnCountPerRound = 5;
    public int maxEnemiesPerRound = 20;
    public int maxConcurrentEnemies = 5;

    [Header("UI & Shop")]
    public GameObject shopUIPanel; // Assign your Shop UI panel here in Inspector  
    public GameObject continueButton; // Assign in Inspector  

    public static Rounds Instance { get; private set; }

    private float roundTimer;
    private float spawnTimer;
    private int currentRound = 0;
    private int enemiesAlive = 0;
    private bool isHordeRound = false;
    private EnemyType hordeEnemyType;
    

    public enum RoundState { Combat, Shop }
    private RoundState currentState = RoundState.Combat;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (rainParticleSystem != null)
            rainParticleSystem.Stop(); // Ensure rain is off at game start

        if (shopUIPanel != null)
            shopUIPanel.SetActive(false);

        StartRound();
    }

    private void Update()
    {
        if (currentState != RoundState.Combat)
            return;

        // Only decrement timer if NOT a horde round
        if (!isHordeRound)
            roundTimer -= Time.deltaTime;

        spawnTimer += Time.deltaTime;

        // End round if timer runs out (normal rounds only)
        if (!isHordeRound && roundTimer <= 0f)
        {
            EndRound();
            return;
        }

        // Only spawn normal enemies if NOT a horde round
        if (!isHordeRound && spawnTimer >= spawnRateDelay && enemiesAlive < maxConcurrentEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    public void StartRound()
    {
        currentRound++;
        UpdateRoundUI();

        roundTimer = roundDuration;
        spawnTimer = 0f;
        spawnRateDelay = Mathf.Max(0.5f, spawnRateDelay - 0.1f);

        // Rain effect logic
        if (rainParticleSystem != null)
        {
            if (currentRound % 5 == 0)
            {
                rainParticleSystem.Play();
                Debug.Log("rain");
                SoundManager.Instance.PlayLoopedSound("Rain");
            }
            else
            {
                rainParticleSystem.Stop();
                SoundManager.Instance.StopLoopedSound();
            }
        }

        if (!doctorKilled && currentRound >= nextDoctorRound)
        {
            SpawnDoctor();
            nextDoctorRound += 3; // Next spawn in 3 rounds
        }

        // Horde round logic
        if (currentRound % 5 == 0)
        {
            isHordeRound = true;
            hordeEnemyType = enemyTypes[Random.Range(0, enemyTypes.Count)];
            Debug.Log($"HORDE ROUND! Spawning {hordeEnemyType.hordeAmount} of {hordeEnemyType.prefab.name}");
            SpawnHorde();
        }
        else
        {
            isHordeRound = false;
        }



        Debug.Log($"Starting Round {currentRound} - Max Concurrent Enemies: {maxConcurrentEnemies}, Spawn Rate: {spawnRateDelay}s");

        maxConcurrentEnemies = Mathf.Min(20, 5 + currentRound * 2);
        currentState = RoundState.Combat;

        // Make sure Shop UI is hidden and time scale is normal  
        if (shopUIPanel != null)
            shopUIPanel.SetActive(false);

        Time.timeScale = 1f;
    }
    private void SpawnDoctor()
    {
        if (doctorPrefab == null || doctorSpawnPoint == null || doctorKilled) return;
        if (currentDoctor != null) return; // Only one doctor at a time

        GameObject doctorObj = Instantiate(doctorPrefab, doctorSpawnPoint.position, Quaternion.identity);
        currentDoctor = doctorObj.GetComponent<DoctorNpc>();
        if (currentDoctor != null)
        {
            currentDoctor.OnDoctorKilled = () => { doctorKilled = true; };
        }
    }

    private void SpawnHorde()
    {
        for (int i = 0; i < hordeEnemyType.hordeAmount; i++)
        {
            Transform spawnPoint = hordeSpawnPoints[Random.Range(0, hordeSpawnPoints.Length)];
            Instantiate(hordeEnemyType.prefab, spawnPoint.position, Quaternion.identity);
            enemiesAlive++;
        }
    }

    void EndRound()
    {
        ClearRemainingEnemies();
        currentState = RoundState.Shop;

        ShowShop();
    }

    void ShowShop()
    {
        Debug.Log("SHOP TIME! Display shop UI here.");

        if (shopUIPanel != null)
            shopUIPanel.SetActive(true);

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ContinueToNextRound()
    {
        if (currentState == RoundState.Shop)
        {
            currentState = RoundState.Combat;

            if (shopUIPanel != null)
                shopUIPanel.SetActive(false);

            if (continueButton != null)
                continueButton.gameObject.SetActive(false);

            Time.timeScale = 1f;
            StartRound();
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyPrefab = GetRandomEnemy();
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.RoundsHealthMultiplier(currentRound);
        }

        enemiesAlive++;
    }
    

    GameObject GetRandomEnemy()
    {
        int totalWeight = 0;
        foreach (var enemy in enemyTypes)
            totalWeight += enemy.weight;

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (var enemy in enemyTypes)
        {
            cumulativeWeight += enemy.weight;
            if (randomValue < cumulativeWeight)
            {
                return enemy.prefab;
            }
        }

        return null;
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
        // End horde round only when all horde zombies are dead
        if (isHordeRound && enemiesAlive == 0)
        {
            EndRound();
        }
    }

    private void ClearRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        enemiesAlive = 0;
    }

    void UpdateRoundUI()
    {
        if (roundText != null)
        {
            roundText.text = "Round: " + currentRound;
        }
    }
}
