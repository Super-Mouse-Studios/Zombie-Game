using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SocialPlatforms.GameCenter;
using Unity.VisualScripting;

public class Shooting : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    public GameObject projectilePrefab;

    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject meleePrefab;
    [SerializeField] GameObject sniperShotPrefab;
    [SerializeField] GameObject shotgunShotPrefab;

    [Header("Shooting")]
    public bool isTriggerDown;
    public float timeUntilReloaded, meleeCooldown = 0;
    public float fireRate = 1; // shots per secend 
    public float meleeRate = 1; // Attacks per second
    // public int shootMode = 1;
    // public float detectionRange = 10f; // Range within which the player can shoot
    public ShootingBehavours shootMode = ShootingBehavours.Basic;
    public ShootingBehavours currentlyHeld = ShootingBehavours.Basic; // Secondary Weapon; If this is swapped, ensure shooting is set back to Basic
    private ShootingAnimation sa;
    [SerializeField] SpriteRenderer sr;

    [Header("Pickups")]
    private float attackSizeLength = 0; // Timer for attack size pickup
    [SerializeField][Range(1.25f, 3f)] float attackSizeMultiplier = 1.5f;
    public float damagePowerUp = 0f;
    [SerializeField][Range(1f, 2f)] float damageMuliplier = 1.5f;

    [Header("Ammo")]
    public int Max_ammo = 500; //max amount of ammos
    private int Current_ammo; //current amount of ammos
    public TMP_Text Ammo_Display; //ammo ui display

    private UnityEngine.Camera mainCam;
    private Vector3 mousePos;
    private Player_Movement pm;

    // Enum for types of shoot modes
    public enum ShootingBehavours
    {
        Basic,
        Spread,
        Rocket,
        AR,
        Sniper,
        Revolver
    }

    private Transform targetEnemy;

    void Start()
    {
        mainCam = UnityEngine.Camera.main;
        Current_ammo = 30;
        sa = GetComponentInChildren<ShootingAnimation>();
        sr = GetComponentInChildren<SpriteRenderer>();
        pm = GetComponentInParent<Player_Movement>();
        sa.PlayAnimation("flicker");
    }

    void Update()
    {
        //amo display
        Ammo_Display.text = "Ammo left: " + Current_ammo.ToString();

        isTriggerDown = Input.GetMouseButton(0) || Input.GetButton("Jump");

        // Melee attack 
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButton(1))
            MeleeAttackBehaviour();

        // Changes current ShootMode 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shootMode = ShootingBehavours.Basic;
            sa.PlayAnimation("flicker");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapToSecondary();

        AimTowardsMouse();

        // Determines what mode to shoot
        switch (shootMode)
        {
            case ShootingBehavours.Basic:
                if (isTriggerDown)
                    BasicShootingBehaviour();
                break;
            case ShootingBehavours.Spread:
                if (isTriggerDown)
                    SpreadShootingBehaviour();
                break;
            case ShootingBehavours.Rocket:
                if (isTriggerDown)
                    RocketShootingBehaviour();
                break;
            case ShootingBehavours.AR:
                if (isTriggerDown)
                    ARShootingBehaviour();
                break;
            case ShootingBehavours.Sniper:
                if (isTriggerDown)
                    SniperShootingBehaviour();
                break;
            case ShootingBehavours.Revolver:
                if (isTriggerDown)
                    RevolverShootingBehaviour();
                break;
        }

        timeUntilReloaded -= Time.deltaTime;
        if (timeUntilReloaded <= 0)
            timeUntilReloaded = 0;

        meleeCooldown -= Time.deltaTime;
        if (meleeCooldown <= 0)
            meleeCooldown = 0;

        // Delete later, just for shoot testing
        if (Input.GetKeyDown(KeyCode.Y))
        {
            shootMode = ShootingBehavours.Spread;
            currentlyHeld = ShootingBehavours.Spread;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            shootMode = ShootingBehavours.Rocket;
            currentlyHeld = ShootingBehavours.Rocket;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            shootMode = ShootingBehavours.AR;
            currentlyHeld = ShootingBehavours.AR;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            shootMode = ShootingBehavours.Sniper;
            currentlyHeld = ShootingBehavours.Sniper;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.RightCommand))
        {
            shootMode = ShootingBehavours.Revolver;
            currentlyHeld = ShootingBehavours.Revolver;
            SwapToSecondary();
        }

        if (attackSizeLength > 0)
                attackSizeLength -= Time.deltaTime;
        if (damagePowerUp > 0)
            damagePowerUp -= Time.deltaTime;
    }

    void AimTowardsMouse()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ - 91f); // Undos the aiming Offset

        // float angleFromHorizontal = Mathf.Abs(Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);

        // // Only flip if the player is aiming more horizontally (not aiming mostly up/down)
        // if (angleFromHorizontal < 60f || angleFromHorizontal > 120f)
        // {
        if (mousePos.x < transform.position.x) // Mouse is to the left of player
        {

            // Aiming left
            if (!pm.lookingRight)
            {
                sr.flipY = false;
            }
            else
            {
                sr.flipY = true;
            }
        }
        else // Mouse is right of player
        {
            // Aiming right
            if (!pm.lookingRight)
            {
                sr.flipY = true;
            }
            else
            {
                sr.flipY = false;
            }
        }
        // }
    }

    void SwapToSecondary()
    {
        switch (currentlyHeld)
        {
            case ShootingBehavours.Spread:
                shootMode = ShootingBehavours.Spread;
                sa.PlayAnimation("flicker 0_3");
                break;
            case ShootingBehavours.Rocket:
                shootMode = ShootingBehavours.Rocket;
                sa.PlayAnimation("flicker 0_5");
                break;
            case ShootingBehavours.AR:
                shootMode = ShootingBehavours.AR;
                sa.PlayAnimation("flicker 0_4");
                break;
            case ShootingBehavours.Sniper:
                shootMode = ShootingBehavours.Sniper;
                sa.PlayAnimation("Flicker");
                break;
            case ShootingBehavours.Revolver:
                shootMode = ShootingBehavours.Revolver;
                sa.PlayAnimation("flicker 0_2");
                break;
            default:
                shootMode = ShootingBehavours.Basic;
                sa.PlayAnimation("flicker");
                break;
        }
    }

    // Basic Shooting Behaviour
    void BasicShootingBehaviour()
    {
        if (timeUntilReloaded <= 0 && Current_ammo > 0)
        {
            sa.PlayShootingAnimation("shot ");
            SoundManager.Instance.PlaySound("Chaingun"); // Plays Chaingun SFX

            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            if (attackSizeLength > 0)
                projectile.transform.localScale *= attackSizeMultiplier;

            float secondsPerShot = 1 / fireRate;
            timeUntilReloaded += secondsPerShot;
            Current_ammo--;
        }
    }

    // Spread Shooting Behaviour
    void SpreadShootingBehaviour()
    {
        if (timeUntilReloaded <= 0 && Current_ammo >= 3)
        {
            sa.PlayShootingAnimation("shoot 0_3");
            SoundManager.Instance.PlaySound("Shotgun"); // Plays Shotgun SFX

            // Center bullet
            GameObject center = Instantiate(shotgunShotPrefab, transform.position, transform.rotation);

            // Left bullet (-12 degrees)
            Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, -12f);
            GameObject left = Instantiate(shotgunShotPrefab, transform.position, leftRotation);

            // Right bullet (+12 degrees)
            Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, 12f);
            GameObject right = Instantiate(shotgunShotPrefab, transform.position, rightRotation);

            if (attackSizeLength > 0)
            {
                center.transform.localScale *= attackSizeMultiplier;
                left.transform.localScale *= attackSizeMultiplier;
                right.transform.localScale *= attackSizeMultiplier;
            }

            float secondsPerShot = 1 / fireRate;
            timeUntilReloaded += secondsPerShot;
            Current_ammo = Current_ammo - 3;
        }
    }

    // Shoots a rocket where projectile will deal damage on impact and cause an explosion which will deal addtional damage to player and zombie, has 1.2x cooldown
    void RocketShootingBehaviour()
    {
        if (timeUntilReloaded <= 0 && Current_ammo > 0)
        {
            sa.PlayShootingAnimation("shoot 0_5");
            SoundManager.Instance.PlaySound("Rocket");

            GameObject rocket = Instantiate(rocketPrefab, transform.position, transform.rotation);

            if (attackSizeLength > 0)
                rocket.transform.localScale *= attackSizeMultiplier;

            float secondsPerShot = 1 / (fireRate / 1.2f);
            timeUntilReloaded += secondsPerShot;
            Current_ammo--;
        }
    }

    // Shoots 4x as fast as basic gun
    void ARShootingBehaviour()
    {
        if (timeUntilReloaded <= 0 && Current_ammo > 0)
        {
            sa.PlayShootingAnimation("shoot 0_4");
            SoundManager.Instance.PlaySound("AR");

            Instantiate(projectilePrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / (fireRate * 4);
            timeUntilReloaded += secondsPerShot;
            Current_ammo--;
        }
    }

    // Sniper shot; has 2x cooldown
    void SniperShootingBehaviour()
    {
        if (timeUntilReloaded <= 0 && Current_ammo > 0)
        {
            sa.PlayShootingAnimation("Shoot");
            SoundManager.Instance.PlaySound("Charger");

            GameObject projectile = Instantiate(sniperShotPrefab, transform.position, transform.rotation);

            if (attackSizeLength > 0)
                projectile.transform.localScale *= attackSizeMultiplier;

            float secondsPerShot = 1 / (fireRate / 2);
            timeUntilReloaded += secondsPerShot;
            Current_ammo--;
        }
    }


    // Melee attack
    void MeleeAttackBehaviour()
    {
        if (meleeCooldown <= 0)
        {
            SoundManager.Instance.PlaySound("Knife"); // Plays knife SFX

            timeUntilReloaded += 0.25f; // So you don't fire while doing a melee attack

            float meleeOffset = .9f; // Melee Offset from player
            Vector3 spawnPosition = transform.position + transform.up * meleeOffset;

            float meleeAngle = -33f; // So animation is more horizontal to player
            Quaternion meleeRotation = transform.rotation * Quaternion.Euler(0, 0, meleeAngle);

            GameObject meleeObj = Instantiate(meleePrefab, spawnPosition, meleeRotation, this.transform);

            // Compensate for parent's scale so meleeObj appears at (1,1,1) in world space
            Vector3 parentScale = transform.lossyScale;
            meleeObj.transform.localScale = new Vector3( // Scales Melee attack to be 2.9x sized
                2.9f / parentScale.x,
                2.9f / parentScale.y,
                2.9f / parentScale.z
            );

            if (attackSizeLength > 0)
                meleeObj.transform.localScale *= attackSizeMultiplier;

            float secondsPerAttack = 1 / meleeRate;
            meleeCooldown = secondsPerAttack;
        }
    }

    public float CalculateDamage(float baseDamage)
    {
        float level = ExperienceManager.Instance.GetCurrentLevel();
        float damage = baseDamage;

        // Damage increased by each level
        damage += level;

        if (damagePowerUp > 0)
            damage *= damageMuliplier;

        Debug.Log($"{damage} damage done: {baseDamage} from base, {level} from levels" + (damagePowerUp > 0 ? $", boosted by x{damageMuliplier}" : ""));
        return damage;
    }

    // Don't put this in the shop
    void RevolverShootingBehaviour() 
    {
        if (timeUntilReloaded <= 0)
        {
            sa.PlayShootingAnimation("shoot 0_2");
            SoundManager.Instance.PlaySound("Shotgun"); // Plays Shotgun SFX

            // Center bullet
            GameObject center = Instantiate(rocketPrefab, transform.position, transform.rotation);

            // Left bullet (-12 degrees)
            Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, -12f);
            GameObject left = Instantiate(sniperShotPrefab, transform.position, leftRotation);

            // Right bullet (+12 degrees)
            Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, 12f);
            GameObject right = Instantiate(sniperShotPrefab, transform.position, rightRotation);

            if (attackSizeLength > 0)
            {
                center.transform.localScale *= attackSizeMultiplier;
                left.transform.localScale *= attackSizeMultiplier;
                right.transform.localScale *= attackSizeMultiplier;
            }

            float secondsPerShot = 1 / (fireRate * 4);
            timeUntilReloaded += secondsPerShot;
            Current_ammo++;
        }
    }

    public void IncreaseAmmo(int ammo) { Current_ammo += ammo; }

    public void AttackSizeTimer(float time = 7) { attackSizeLength = time; }

    public void DamageUp(float time = 7f) { damagePowerUp = time; }
}

    // Old Auto Aim Code
    // void AutoAimandShoot()
    // {
    //     FindNearestEnemy();

    //     if (targetEnemy != null)
    //     {
    //         // Calculate direction in 2D
    //         Vector2 direction = (targetEnemy.position - transform.position).normalized;
    //         // Calulate angle and rotate only around Z axis
    //         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //         transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

    //         //Shoot if reloaded
    //         switch (shooting)
    //         {
    //             case ShootingBehavours.Basic:
    //                 BasicShootingBehaviour();
    //                 break;
    //             case ShootingBehavours.Spread:
    //                 SpreadShootingBehaviour();
    //                 break;
    //             case ShootingBehavours.Rocket:
    //                 RocketShootingBehaviour();
    //                 break;
    //             case ShootingBehavours.AR:
    //                 ARShootingBehaviour();
    //                 break;
    //             case ShootingBehavours.Sniper:
    //                 SniperShootingBehaviour();
    //                 break;
    //         }
    //     }
    // }

    // void FindNearestEnemy()
    // {
    //     GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //     float closestDistance = detectionRange;
    //     targetEnemy = null;

    //     foreach (GameObject enemy in enemies)
    //     {
    //         float distance = Vector3.Distance(transform.position, enemy.transform.position);
    //         if (distance < closestDistance)
    //         {
    //             closestDistance = distance;
    //             targetEnemy = enemy.transform;
    //         }
    //     }
    // }