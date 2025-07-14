using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject meleePrefab;
    [SerializeField] GameObject sniperShotPrefab;
    public bool isTriggerDown;
    public float timeUntilReloaded, meleeCooldown = 0;
    public float fireRate = 1; // shots per secend 
    public float meleeRate = 1; // Attacks per second
    public int shootMode = 1;
    public float detectionRange = 10f; // Range within which the player can shoot
    public ShootingBehavours shooting = ShootingBehavours.Basic;
    private UnityEngine.Camera mainCam;
    private Vector3 mousePos;

    // Enum for types of shoot modes
    public enum ShootingBehavours
    {
        Basic,
        Spread,
        Rocket,
        AR,
        Sniper
    }

    private Transform targetEnemy;

    void Start()
    {
        mainCam = UnityEngine.Camera.main;
    }

    void Update()
    {
        isTriggerDown = Input.GetMouseButton(0) || Input.GetButton("Jump");

        // Melee attack 
        if (Input.GetKeyDown(KeyCode.F))
            MeleeAttackBehaviour();

        // Changes current ShootMode for testing purposes; comment out later
        if (Input.GetKeyDown(KeyCode.Alpha1))
            shooting = ShootingBehavours.Basic;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            shooting = ShootingBehavours.Spread;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            shooting = ShootingBehavours.Rocket;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            shooting = ShootingBehavours.AR;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            shooting = ShootingBehavours.Sniper;

        AimTowardsMouse();
        
        // Determines what mode to shoot
        switch (shooting)
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
        }

        timeUntilReloaded -= Time.deltaTime;
        if (timeUntilReloaded <= 0)
            timeUntilReloaded = 0;

        meleeCooldown -= Time.deltaTime;
        if (meleeCooldown <= 0)
            meleeCooldown = 0;
    }

    void AimTowardsMouse()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ - 91f);
    }

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

    // Basic Shooting Behaviour
    void BasicShootingBehaviour()
    {
        if (timeUntilReloaded <= 0)
        {
            SoundManager.Instance.PlaySound("Chaingun"); // Plays Chaingun SFX

            Instantiate(projectilePrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / fireRate;
            timeUntilReloaded += secondsPerShot;
        }
    }

    // Spread Shooting Behaviour
    void SpreadShootingBehaviour()
    {
        if (timeUntilReloaded <= 0)
        {
            SoundManager.Instance.PlaySound("Shotgun"); // Plays Shotgun SFX

            // Center bullet
            Instantiate(projectilePrefab, transform.position, transform.rotation);

            // Left bullet (-12 degrees)
            Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, -12f);
            Instantiate(projectilePrefab, transform.position, leftRotation);

            // Right bullet (+12 degrees)
            Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, 12f);
            Instantiate(projectilePrefab, transform.position, rightRotation);

            float secondsPerShot = 1 / fireRate;
            timeUntilReloaded += secondsPerShot;
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

            float secondsPerAttack = 1 / meleeRate;
            meleeCooldown = secondsPerAttack;
        }
    }

    // Shoots a rocket where projectile will deal damage on impact and cause an explosion which will deal addtional damage to player and zombie, has 1.2x cooldown
    void RocketShootingBehaviour()
    {
        if (timeUntilReloaded <= 0)
        {
            SoundManager.Instance.PlaySound("Rocket");

            Instantiate(rocketPrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / (fireRate / 1.2f);
            timeUntilReloaded += secondsPerShot;
        }
    }

    // Shoots 4x as fast as basic gun
    void ARShootingBehaviour()
    {
        if (timeUntilReloaded <= 0)
        {
            SoundManager.Instance.PlaySound("AR");

            Instantiate(projectilePrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / (fireRate * 4);
            timeUntilReloaded += secondsPerShot;
        }
    }

    // Sniper shot; has 2x cooldown
    void SniperShootingBehaviour()
    {
        if (timeUntilReloaded <= 0)
        {
            SoundManager.Instance.PlaySound("Charger");
            
            Instantiate(sniperShotPrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / (fireRate / 2);
            timeUntilReloaded += secondsPerShot;
        } 
    }
}