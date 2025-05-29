using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 8f;
    public float range = 25;
    public float distanceTravelled = 0;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //checking if collision have enemy component on it
        if (collision.gameObject.TryGetComponent<Zombie_Following>(out Zombie_Following zombieComponent))
        {
            //zombie taking damage
            zombieComponent.EnemyTakeDamage(5);
            
            //destroying bullet after hitting
            Destroy(gameObject);
        }
        

    }
    
    void Update()
    {
        //bullet moving forward
        float dt = Time.deltaTime;
        Vector3 forwardVector = transform.up; 
        transform.position = transform.position + forwardVector * speed * dt;

        distanceTravelled += speed * dt;
        
        //destroying bullet after a range
        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }
    }
}
