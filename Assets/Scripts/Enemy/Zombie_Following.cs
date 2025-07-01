using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Following : MonoBehaviour
{
    // Add this event for death notification
    public event Action OnDeath;

    [SerializeField] private float enemyHealth, enemyMaxhealth = 5f;

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

    //enemy taking damage and dying
    public void EnemyTakeDamage(float enemyDamageAmount)
    {
        enemyHealth -= enemyDamageAmount;
        if (enemyHealth <= 0)
        {
            GameManager.instance.ZombieDied();
            OnDeath?.Invoke(); // Notify listeners (Rounds.cs) before destroying
            Destroy(gameObject);
        }
    }


    [SerializeField]
    private float speed;
    [SerializeField]
    private float roatationSpeed;
    [SerializeField]
    private float _screenBorder;

    //Basic obsticle avoidness
    [SerializeField]
    private float _obstacleCheckCircleRadius;
    [SerializeField]
    private float _obstacleCheckDistance;
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    private Rigidbody2D rigidbody;
    private PlayerAwareness playerAwareness;
    private Vector2 targetDirection;
    private float changeDirectionCooldown;
    private UnityEngine.Camera unityMainCamera;
    private RaycastHit2D[] obstacleCollisions;
    private float obstacleAvoidanceCooldown;
    private Vector2 obstacleAvoidanceTargetDirection;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerAwareness = GetComponent<PlayerAwareness>();
        targetDirection = transform.up;
        unityMainCamera = UnityEngine.Camera.main;
        obstacleCollisions = new RaycastHit2D[10];
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
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

        rigidbody.SetRotation(rotation);
    }
    private void SetVelocity()
    {
        if (targetDirection == Vector2.zero)
        {
            rigidbody.velocity = Vector2.zero;
        }
        else
        {
            rigidbody.velocity = transform.up * speed;
        }
    }
}

