using System.Collections;
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
    private float gasedUp = 0f;

    [Header("Animation")]
    public Animator animator;
    [SerializeField] SpriteRenderer sr;
    public bool lookingRight = false;
    private bool hitPlaying = false;

    public enum MovementState // Defines movements player can take
    {
        Normal,
        Dodging
    }

    void ApplyUpgrades()
    {
        // Apply max HP upgrade
        maxHealth += PlayerStatUpgrades.Instance.maxHPUpgrade;

        // Apply other upgrades similarly...

      
    }


    void Start()
    {
        ExperienceManager.Instance.ResetLevels();
        ExperienceManager.Instance.ShowText();
        //health status at the start of the game
        currentHealth = maxHealth;
        healthbar.updateHealthBar(maxHealth + PlayerStatUpgrades.Instance.maxHPUpgrade, currentHealth); //initial update with upgrades

        // Saves original dodge speed/cooldown so it lines up if it's changed in inspector
        originalDodgeSpeed = dodgeSpeed;

        hurtbox = GetComponent<CapsuleCollider2D>();

        ApplyUpgrades();

        // Just for debug, print the max health after upgrades applied
        Debug.Log($"[DEBUG] Max Health after upgrades: {maxHealth}");
    }

    //player taking damage and dying
    public void PlayerTakeDamage(float PlayerDamageAmount)
    {
        SoundManager.Instance.PlaySound("PlayerHurt");
        currentHealth -= PlayerDamageAmount;
        // Clamp health so it doesn't go above max + upgrade
        float effectiveMaxHealth = maxHealth + PlayerStatUpgrades.Instance.maxHPUpgrade;
        currentHealth = Mathf.Min(currentHealth, effectiveMaxHealth);

        healthbar.updateHealthBar(effectiveMaxHealth, currentHealth);

        if (currentHealth <= 0)
        {
            ExperienceManager.Instance.HideText();
            Destroy(gameObject);
        }

        StopAllCoroutines();
        StartCoroutine(PlayDamageAnimation());
    }

    public IEnumerator PlayDamageAnimation()
    {
        animator.Play("hit");
        hitPlaying = true;
        yield return new WaitForSeconds(.3f);
        animator.Play("idle");
        hitPlaying = false;
    }

    void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        inputvector = new Vector3(inputX, inputY, 0);

        // Calculate effective stats by adding base + upgrades
        float effectiveSpeed = speed + PlayerStatUpgrades.Instance.movementSpeedUpgrade;
        float effectiveDodgeSpeed = dodgeSpeed + PlayerStatUpgrades.Instance.dodgeSpeedUpgrade;
        float effectiveDodgeCooldown = Mathf.Max(0f, dodgeCooldown - PlayerStatUpgrades.Instance.dodgeCooldownReduction);
        float effectiveMaxHealth = maxHealth + PlayerStatUpgrades.Instance.maxHPUpgrade;

        // Handles movement based on which state player is in
        switch (state)
        {
            case MovementState.Normal: // Normal walking state
                if (!(inputvector.x == 0 && inputvector.y == 0))
                {
                    float dt = Time.deltaTime;
                    Vector3 direction = inputvector.normalized;
                    transform.position += inputvector / inputvector.magnitude * effectiveSpeed * dt;
                    transform.position += direction * effectiveSpeed * dt;

                    Vector3 scale = transform.localScale;
                    if (inputvector.x > 0) // when moving right
                    {
                        scale.x = -Mathf.Abs(scale.x);
                        lookingRight = true;
                    }
                    else
                    {
                        scale.x = Mathf.Abs(scale.x);
                        lookingRight = false;
                    }

                    transform.localScale = scale;
                    if (!hitPlaying)
                        animator.Play("run cycle ");
                }
                else
                {
                    if (!hitPlaying)
                        animator.Play("idle");
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeCooldownReload <= 0) // Switches to dodge state
                {
                    state = MovementState.Dodging;
                    Quaternion prefabRotation = transform.rotation;

                    if (inputvector == Vector3.zero && dodgeDirection == new Vector3(-1111, -2222, -3333)) // Fixes orientation if player is not moving
                        prefabRotation = transform.rotation * Quaternion.AngleAxis(180, Vector3.forward);

                    GameObject justSpawned = Instantiate(dodgeEffect, transform.position, prefabRotation); // Spawns in particle effect under player's original position
                    justSpawned.transform.localScale *= 1.3f;

                    Destroy(justSpawned, .5f);
                    // Plays SFX
                    SoundManager.Instance.PlaySound("Dash");
                }
                break;

            case MovementState.Dodging: // Dodge state
                DodgeRoll(effectiveDodgeSpeed);
                break;
        }

        // Reset dodge cooldown over time using effective cooldown
        if (gasedUp > 0)
        {
            gasedUp -= Time.deltaTime;
            if (effectiveDodgeCooldown > 0)
                dodgeCooldownReload = 0;
        }
        else if (dodgeCooldownReload > 0)
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

    // Player Level Up Health increase (optional, depends on how you want to handle leveling)
    public void LevelUp()
    {
        ++maxHealth;
        ++currentHealth;
        Debug.Log($"MaxHP: {maxHealth}, Current HP: {currentHealth}");

        float effectiveMaxHealth = maxHealth + PlayerStatUpgrades.Instance.maxHPUpgrade;
        healthbar.updateHealthBar(effectiveMaxHealth, currentHealth);
    }

    // Dodge roll method now takes effective dodge speed
    private void DodgeRoll(float effectiveDodgeSpeed)
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

        transform.position += dodgeDirection * effectiveDodgeSpeed * Time.deltaTime;

        // Decelerates speed
        effectiveDodgeSpeed -= effectiveDodgeSpeed * 10f * Time.deltaTime;

        // Switches back to normal state after speed decelerates enough and resets dodge speed to original
        if (effectiveDodgeSpeed <= 5f)
        {
            state = MovementState.Normal;
            dodgeCooldownReload = Mathf.Max(0f, dodgeCooldown - PlayerStatUpgrades.Instance.dodgeCooldownReduction);

            hurtbox.enabled = true; // Re-enables hurtbox at the end of dodges
        }
    }

    public void Gasoline(float time = 7f) { gasedUp = time; } // How long you're gased up for
    public void Healing(int heal = 1)
    {
        currentHealth += heal;
        float effectiveMaxHealth = maxHealth + PlayerStatUpgrades.Instance.maxHPUpgrade;
        currentHealth = Mathf.Min(currentHealth, effectiveMaxHealth);
        healthbar.updateHealthBar(effectiveMaxHealth, currentHealth);
    }
}