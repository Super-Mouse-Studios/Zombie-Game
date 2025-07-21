using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Following : MonoBehaviour
{
    [System.Serializable]
    public struct PickupDrop
    {
        public string name;
        public GameObject prefab;
        [Range(0f, 1f)] public float chance;
    }

    // Add this event for death notification
    public event Action OnDeath;

    [SerializeField]
    private float enemyHealth, enemyMaxhealth = 5f;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float roatationSpeed;
    [SerializeField]
    private float _screenBorder;
    [SerializeField]
    private float _obstacleCheckCircleRadius;
    [SerializeField]
    private float _obstacleCheckDistance;
    [SerializeField]
    private LayerMask _obstacleLayerMask;


    [SerializeField]
    private bool isExploder = false; // Set in Inspector per prefab
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionDamage = 15f;
    [SerializeField]
    private GameObject explosionEffectPrefab;

    [SerializeField]
    private bool isToxic = false; // Set in Inspector per prefab

    [SerializeField]
    private float toxicRadius = 3f;
    [SerializeField]
    private float poisonTickDamage = 2f;
    [SerializeField]
    private float poisonTickInterval = 1f;
    [SerializeField]
    private float poisonDuration = 3f;

    [SerializeField]
    private GameObject toxicProjectilePrefab;
    [SerializeField]
    private float shootInterval = 2f;
    [SerializeField]
    private float projectileSpeed = 8f;

    private float poisonTickTimer = 0f;
    private float shootTimer = 0f;



    private Rigidbody2D rigidbody2d;
    private PlayerAwareness playerAwareness;
    private Vector2 targetDirection;
    private float changeDirectionCooldown;
    private UnityEngine.Camera unityMainCamera;
    private RaycastHit2D[] obstacleCollisions;
    private float obstacleAvoidanceCooldown;
    private Vector2 obstacleAvoidanceTargetDirection;

    // // Power-up related variables
    // [SerializeField]
    // public GameObject fireRatePowerUpPrefab;
    // [SerializeField] GameObject ammoPickupPrefab;
    // [SerializeField] GameObject gasolinePrefab;

    // [Range(0f, 1f)] public float fireRatePowerUpChance = 0.10f; // 10% chance to drop a fire rate power-up on death
    // [SerializeField][Range(0f,1f)] float ammoPickupChance = .25f;
    // [SerializeField][Range(0f,1f)] float gasolineChance = .075f;


    [SerializeField] PickupDrop[] pickupDrops;

    private void Start()
    {
        enemyHealth = enemyMaxhealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player_Movement>(out Player_Movement player))
        {
            player.PlayerTakeDamage(1);
        }
    }
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        playerAwareness = GetComponent<PlayerAwareness>();
        targetDirection = transform.up;
        unityMainCamera = UnityEngine.Camera.main;
        obstacleCollisions = new RaycastHit2D[10];
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        //RotateTowardsTarget();
        SetVelocity();

        if (isToxic)
        {
            ApplyPoisonAura();
            ToxicShoot();
        }
    }

    private void UpdateTargetDirection()
    {
        HandleRandomDirectionChange();
        HandlePlayerTargeting();
        HandleObstacles();
        HandleEnemyOffScreen();
    }
    private void HandleRandomDirectionChange()
    {
        changeDirectionCooldown -= Time.deltaTime;

        if (changeDirectionCooldown <= 0)
        {
            float angleChange = UnityEngine.Random.Range(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            targetDirection = rotation * targetDirection;

            changeDirectionCooldown = UnityEngine.Random.Range(1f, 5f);
        }
    }
    private void HandlePlayerTargeting()
    {
        if (playerAwareness.AwareOfPlayer)
        {
            targetDirection = playerAwareness.DirectionToPlayer;
        }
    }
    private void HandleEnemyOffScreen()
    {
        Vector2 screenPosition = unityMainCamera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < _screenBorder && targetDirection.x < 0) ||
            (screenPosition.x > unityMainCamera.pixelWidth - _screenBorder && targetDirection.x > 0))
        {
            targetDirection = new Vector2(-targetDirection.x, targetDirection.y);
        }

        if ((screenPosition.y < _screenBorder && targetDirection.y < 0) ||
            (screenPosition.y > unityMainCamera.pixelHeight - _screenBorder && targetDirection.y > 0))
        {
            targetDirection = new Vector2(targetDirection.x, -targetDirection.y);
        }
    }
    private void HandleObstacles()
    {
        obstacleAvoidanceCooldown -= Time.deltaTime;

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(_obstacleLayerMask);

        int numberOfCollisions = Physics2D.CircleCast(
            transform.position,
            _obstacleCheckCircleRadius,
            transform.up,
            contactFilter,
            obstacleCollisions,
            _obstacleCheckDistance);

        for (int index = 0; index < numberOfCollisions; index++)
        {
            var obstacleCollision = obstacleCollisions[index];

            if (obstacleCollision.collider.gameObject == gameObject)
            {
                continue;
            }

            if (obstacleAvoidanceCooldown <= 0)
            {
                obstacleAvoidanceTargetDirection = obstacleCollision.normal;
                obstacleAvoidanceCooldown = 0.5f;
            }

            var targetRotation = Quaternion.LookRotation(transform.forward, obstacleAvoidanceTargetDirection);
            var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, roatationSpeed * Time.deltaTime);

            targetDirection = rotation * Vector2.up;
            break;
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetDirection == Vector2.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, roatationSpeed * Time.deltaTime);

        rigidbody2d.SetRotation(rotation);
    }
    private void SetVelocity()
    {
        if (targetDirection == Vector2.zero)
        {
            rigidbody2d.velocity = Vector2.zero;
        }
        else
        {
            rigidbody2d.velocity = targetDirection.normalized * speed;
        }
    }

    private void Explode()
    {
        // Optional: spawn explosion effect
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Damage all Player_Movement objects in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Player_Movement player = hit.GetComponent<Player_Movement>();
            if (player != null)
            {
                player.PlayerTakeDamage(explosionDamage);
            }
        }
        // You can also damage other enemies or objects if needed
    }
    private void ApplyPoisonAura()
    {
        poisonTickTimer -= Time.fixedDeltaTime;
        if (poisonTickTimer <= 0f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, toxicRadius);
            foreach (var hit in hits)
            {
                Player_Movement player = hit.GetComponent<Player_Movement>();
                if (player != null)
                {
                    // Apply poison effect (damage over time)
                    PoisonEffect existing = player.GetComponent<PoisonEffect>();
                    if (existing == null)
                    {
                        PoisonEffect effect = player.gameObject.AddComponent<PoisonEffect>();
                        effect.Initialize(poisonTickDamage, poisonTickInterval, poisonDuration);
                    }
                    else
                    {
                        // Refresh poison duration if already poisoned
                        existing.Initialize(poisonTickDamage, poisonTickInterval, poisonDuration);
                    }
                }
            }
            poisonTickTimer = poisonTickInterval;
        }
    }
    private void ToxicShoot()
    {
        shootTimer -= Time.fixedDeltaTime;
        if (shootTimer <= 0f && playerAwareness != null && playerAwareness.AwareOfPlayer)
        {
            if (toxicProjectilePrefab != null)
            {
                Vector2 direction = playerAwareness.DirectionToPlayer.normalized;
                GameObject proj = Instantiate(toxicProjectilePrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = direction * projectileSpeed;

                // Optionally, set poison values on the projectile if needed
                ToxicProjectile toxic = proj.GetComponent<ToxicProjectile>();
                if (toxic != null)
                    toxic.SetPoison(poisonTickDamage, poisonTickInterval, poisonDuration);
            }
            shootTimer = shootInterval;
        }
    }

    //enemy taking damage and dying
    public void EnemyTakeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;
        Debug.Log($"{name} took {damageAmount} damage. Current health: {enemyHealth}");

        if (enemyHealth <= 0)
        {
            Die();
        }
    }


    public void Die()
    {
        Debug.Log($"{name} has died.");
        if (isExploder)
        {
            Explode();
        }

        Rounds rounds = FindObjectOfType<Rounds>();
        if (rounds != null)
            rounds.EnemyDied();

        GameManager.instance.ZombieDied();
        ExperienceManager.Instance.AddExperience(UnityEngine.Random.Range(4, 9));
        SoundManager.Instance.PlaySound("ZombieDeath");

        SpawnPickup();
        Destroy(gameObject);
    }

    void SpawnPickup()
    {
        foreach (PickupDrop drop in pickupDrops)
        {
            if (drop.prefab != null && UnityEngine.Random.value < drop.chance)
            {
                Instantiate(drop.prefab, transform.position, Quaternion.identity);
                Debug.Log($"{drop.name} dropped");
            }
        }
    }
}

