using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAttack : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab; // Explosion that is caused after impact
    [SerializeField] float speed = 6;
    [SerializeField] float range = 20;
    [SerializeField] float distanceTravelled = 0;
    public float baseDamage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            float damage = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooting>().CalculateDamage(baseDamage);
            enemyHealth.TakeDamage(damage);

            // Spawns in Explosion
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject); // Destroy bullet after hitting an enemy
        }
    }


    // Update is called once per frame
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
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
