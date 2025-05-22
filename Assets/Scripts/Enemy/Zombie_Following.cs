using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Following : MonoBehaviour
{

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

