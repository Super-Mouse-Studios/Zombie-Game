using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

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
    private float dodgeDuration = 0.2f; // Adjust this for dodge length
    private float dodgeTimer = 0f;
    private float originalDodgeSpeed =0f;
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
    [SerializeField] GameObject textPrefab;

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
            PlayerStatUpgrades.Instance.ResetUpgrades();
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
            case MovementState.Normal:
                // Movement logic
                if (!(inputvector.x == 0 && inputvector.y == 0))
                {
                    float dt = Time.deltaTime;
                    Vector3 direction = inputvector.normalized;
                    transform.position += direction * effectiveSpeed * dt;

                    Vector3 scale = transform.localScale;
                    if (inputvector.x > 0)
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

                // Cooldown countdown
                dodgeCooldownReload -= Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeCooldownReload <= 0f)
                {
                    state = MovementState.Dodging;
                    dodgeTimer = dodgeDuration;
                    dodgeCooldownReload = dodgeCooldown;

                    dodgeDirection = inputvector != Vector3.zero
                        ? inputvector.normalized
                        : -transform.up;

                    hurtbox.enabled = false;

                    Quaternion prefabRotation = transform.rotation;
                    if (inputvector == Vector3.zero)
                        prefabRotation = transform.rotation * Quaternion.AngleAxis(180, Vector3.forward);

                    GameObject justSpawned = Instantiate(dodgeEffect, transform.position, prefabRotation);
                    justSpawned.transform.localScale *= 1.3f;
                    Destroy(justSpawned, 0.5f);

                    SoundManager.Instance.PlaySound("Dash");
                }

                break;

            case MovementState.Dodging:
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
        if (dodgeTimer > 0f)
        {
            transform.position += dodgeDirection * effectiveDodgeSpeed * Time.deltaTime;
            dodgeTimer -= Time.deltaTime;
        }
        else
        {
            state = MovementState.Normal;
            dodgeDirection = new Vector3(-1111, -2222, -3333); // Reset sentinel
            hurtbox.enabled = true;
        }
    }

    public void Gasoline(float time = 7f)
    {
        gasedUp = time; // How long you're gased up for
        
        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText("No Dodge Cooldown");
    }
    public void Healing(int heal = 1)
    {
        currentHealth += heal;
        float effectiveMaxHealth = maxHealth + PlayerStatUpgrades.Instance.maxHPUpgrade;
        currentHealth = Mathf.Min(currentHealth, effectiveMaxHealth);
        healthbar.updateHealthBar(effectiveMaxHealth, currentHealth);

        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"+{heal} HP");
        text.color = Color.green;
    }
}