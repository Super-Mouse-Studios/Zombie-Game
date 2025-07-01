using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public bool isTriggerDown;
    public float timeUntilReloaded = 0;
    public float fireRate = 1; // shots per secend 
    public int shootMode = 1;
    public float detectionRange = 10f; // Range within which the player can shoot
    public ShootingBehavours shooting = ShootingBehavours.Basic;
    
    // Enum for types of shoot modes
    public enum ShootingBehavours
    {
        Basic,
        Spread
    }

    private Transform targetEnemy;

    void Update()
    {
        isTriggerDown = Input.GetButtonDown("Jump");
        if (Input.GetKeyDown(KeyCode.E))
        {
            shootMode = 1;

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            shootMode = 2;
        }

        // Changes current ShootMode for testing purposes; comment out later
        if (Input.GetKeyDown(KeyCode.Alpha1))
            shooting = ShootingBehavours.Basic;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            shooting = ShootingBehavours.Spread;


        if (shootMode == 1)
        {
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
            }
        }
        else if (shootMode == 2)
        {
            AutoAimandShoot();
        }

        timeUntilReloaded -= Time.deltaTime;
        if (timeUntilReloaded <= 0)
        {
            timeUntilReloaded = 0;
        }
    }

    void AutoAimandShoot()
    {
        FindNearestEnemy();

        if (targetEnemy != null)
        {
            // Calculate direction in 2D
            Vector2 direction = (targetEnemy.position - transform.position).normalized;
            // Calulate angle and rotate only around Z axis
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            //Shoot if reloaded
            switch (shooting)
            {
                case ShootingBehavours.Basic:
                    BasicShootingBehaviour();
                    break;
                case ShootingBehavours.Spread:
                    SpreadShootingBehaviour();
                    break;
            }
        }
    }
    
    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = detectionRange;
        targetEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetEnemy = enemy.transform;
            }
        }
    }


    void BasicShootingBehaviour()
    {

        if (timeUntilReloaded <= 0)
        {
            Instantiate(projectilePrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / fireRate;
            timeUntilReloaded += secondsPerShot;
        }
    }

    void SpreadShootingBehaviour()
    {
        if (timeUntilReloaded <= 0)
        {
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
}