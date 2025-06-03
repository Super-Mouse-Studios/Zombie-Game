using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Following : MonoBehaviour
{

    //kill status
    // public static event Action<Zombie_Following> onZombieKilled;
    //health status
    [SerializeField] private float enemyHealth, enemyMaxhealth = 5f;


    private void Start()
    {
        //reset the enemy health to max everytime we play the game
        enemyHealth = enemyMaxhealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player_Movement>(out Player_Movement player))
        {
            //Player taking damage
            player.PlayerTakeDamage(1);
        }
    }

    
    //enemy taking damage and dying
    public void EnemyTakeDamage(float enemyDamageAmount)
    {
        enemyHealth -= enemyDamageAmount; //10 -> 9 -> 8 -> 7 -> 6 -> 5 -> 4 -> 3 -> 2 -> 1 -> 0
        
        if (enemyHealth <= 0)
        {
            GameManager.instance.ZombieDied();
            ExperienceManager.Instance.AddExperience(UnityEngine.Random.Range(4, 9)); // Random EXP between 4 and 9
            
            // onZombieKilled?.Invoke(this); //adding the kill number
            Destroy(gameObject);

        }
    }


    [SerializeField]
    private float speed;
    [SerializeField]
    private float roatationSpeed;

    private Rigidbody2D rigidbody;
    private PlayerAwareness playerAwareness;
    private Vector2 targetDirection;

    // Start is called before the first frame update
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerAwareness = GetComponent<PlayerAwareness>();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        if (playerAwareness.AwareOfPlayer)
        {
            targetDirection = playerAwareness.DirectionToPlayer;
        }
        else
        {
            targetDirection = Vector2.zero;
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

