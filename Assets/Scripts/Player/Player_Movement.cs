using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//**************************************
//**this script is for player movement**
//**************************************
public class Player_Movement : MonoBehaviour
{
    //health status
    [Header("Health")]
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Healthbar healthbar;

    // Dodge Stuff
    [Header("Dodge")]
    [SerializeField] GameObject dodgeEffect; // particle effect for dodges
    [SerializeField] float dodgeSpeed = 100f; // Dodge Speed of player; Affects length
    [SerializeField] float dodgeCooldown = 1f; // Cooldown starts after entire dash is played, Adjust accordingly
    [SerializeField] float dodgeCooldownReload = 0;
    private float originalDodgeSpeed;
    private Vector3 dodgeDirection = new Vector3(-1111, -2222, -3333);

    [Header("Map Boundaries")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    public float speed = 5f; // Speed of the player
    private CapsuleCollider2D hurtbox;
    public Vector3 inputvector = Vector3.zero;
    private Rigidbody2D rb;
    private MovementState state = MovementState.Normal;

    public Animator animator;

    public enum MovementState // Defines movements player can take
    {
        Normal,
        Dodging
    }

    void Start()
    {
        //health status at the start of the game
        currentHealth = maxHealth;
        healthbar.updateHealthBar(maxHealth, currentHealth); //updating health bar

        // Saves original dodge speed/cooldown so it lines up if it's changed in inspector
        originalDodgeSpeed = dodgeSpeed;

        hurtbox = GetComponent<CapsuleCollider2D>();

    }

    //player taking damage and dying
    public void PlayerTakeDamage(float PlayerDamageAmount)
    {
        currentHealth -= PlayerDamageAmount; //10 -> 9 -> 8 -> 7 -> 6 -> 5 -> 4 -> 3 -> 2 -> 1 -> 0
        healthbar.updateHealthBar(maxHealth, currentHealth); //updating health bar
        animator.Play("Player_Hurt"); // Plays hurt animation
        if (currentHealth <= 0)
        {
            SSmanager.LoadGameOverScene(); // Load Game Over scene
            Destroy(gameObject);
        }
    }



    void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        inputvector = new Vector3(inputX, inputY, 0);

        // Handles movement based on which state player is in
        switch (state)
        {
            case MovementState.Normal: // Normal walking state
                if (!(inputvector.x == 0 && inputvector.y == 0))
                {

                    float dt = Time.deltaTime;
                    Vector3 direction = inputvector.normalized;
                    transform.position += inputvector / inputvector.magnitude * speed * dt;
                    transform.position += direction * speed * dt;
                    //Vector3 rotatedDirection = new Vector3(-direction.x, direction.y, 0);
                    //transform.up = direction;
                    // animator.SetBool("Move", true); // Sets walking animation
                    animator.Play("run cycle ");
                }
                else
                {
                    // animator.SetBool("Move", false); // Stops walking animation
                    animator.Play("idle");
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeCooldownReload <= 0) // Switches to dodge state
                {
                    state = MovementState.Dodging;
                    Quaternion prefabRotation = transform.rotation;

                    if (inputvector == Vector3.zero && dodgeDirection == new Vector3(-1111, -2222, -3333)) // Fixes orientation if player is not moving
                        prefabRotation = transform.rotation * Quaternion.AngleAxis(180, Vector3.forward);

                    GameObject justSpawned = Instantiate(dodgeEffect, transform.position, prefabRotation); // Spawns in particle effect under player's original position

                    Destroy(justSpawned, .5f);
                    // Plays SFX
                    SoundManager.Instance.PlaySound("Dash");
                }
                break;

            case MovementState.Dodging: // Dodge state
                DodgeRoll();
                break;
        }

        // Reset dodge cooldown over time
        if (dodgeCooldownReload >= 0)
        {
            dodgeCooldownReload -= Time.deltaTime;

            if (dodgeCooldownReload <= 0)
                dodgeCooldownReload = 0;
        }
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }

    // Player Level Up Health increase
    public void LevelUp()
    {
        ++maxHealth;
        ++currentHealth;
        Debug.Log($"MaxHP: {maxHealth}, Current HP: {currentHealth}");

        healthbar.updateHealthBar(maxHealth, currentHealth);
    }


    // Dodge roll method
    private void DodgeRoll()
    {
        Debug.Log("Now Dodging");

        hurtbox.enabled = false; // Disables hurtbox

        if (inputvector != Vector3.zero && dodgeDirection == new Vector3(-1111, -2222, -3333)) // Dodge in the direction player is walking
            dodgeDirection = inputvector.normalized;
        else // Dodge away from player
        {
            if (dodgeDirection == new Vector3(-1111, -2222, -3333)) // Prevents changing directions in the middle of a roll
                dodgeDirection = -transform.up;
        }

        transform.position += dodgeDirection * dodgeSpeed * Time.deltaTime;

        dodgeSpeed -= dodgeSpeed * 10f * Time.deltaTime; // Decelerates speed

        // Switches back to normal state after speed decelerates enough and resets dodge speed to original
        if (dodgeSpeed <= 5f)
        {
            state = MovementState.Normal;
            dodgeSpeed = originalDodgeSpeed;
            dodgeDirection = new Vector3(-1111, -2222, -3333); // Sets dodge direction to an impossible vector
            dodgeCooldownReload = dodgeCooldown;

            hurtbox.enabled = true; // Re-enables hurtbox at the end of dodges
        }
    }
}