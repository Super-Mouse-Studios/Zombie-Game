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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isTriggerDown = Input.GetButtonDown("Jump");
        if (Input.GetKeyDown(KeyCode.E))
        {
            shootMode = 1;

        }
     
        if (shootMode == 1)
        {
            BasicShootingBehaviour();
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

