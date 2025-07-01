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

        if (shootMode == 1)
        {
            BasicShootingBehaviour();
        }
        else if (shootMode == 2)
        {
            AutoAimandShoot();
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
            if (timeUntilReloaded <= 0)
            {
                Instantiate(projectilePrefab, transform.position, transform.rotation);
                float secondsPerShot = 1 / fireRate;
                timeUntilReloaded += secondsPerShot;
            }
        }

        timeUntilReloaded -= Time.deltaTime;
        if (timeUntilReloaded <= 0)
        {
            timeUntilReloaded = 0;
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

        if (isTriggerDown && timeUntilReloaded <= 0)
        {
            Instantiate(projectilePrefab, transform.position, transform.rotation);
         
            float secondsPerShot = 1 / fireRate;
            timeUntilReloaded += secondsPerShot;
        }

        timeUntilReloaded -= Time.deltaTime;
        if (timeUntilReloaded <= 0)
        {
            timeUntilReloaded = 0;
        }

    }

 
}