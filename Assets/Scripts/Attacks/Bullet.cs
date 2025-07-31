using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public float range = 25;
    public float distanceTravelled = 0;
    public float baseDamage = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            float damage = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shooting>().CalculateDamage(baseDamage);
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet after hitting an enemy
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
