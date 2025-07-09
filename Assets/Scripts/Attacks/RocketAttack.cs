using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAttack : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab; // Explosion that is caused after impact
    [SerializeField] float speed = 6;
    [SerializeField] float range = 20;
    [SerializeField] float distanceTravelled = 0;
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Calculate damage based on level
            damage = CalculateDamage();
            enemyHealth.TakeDamage(damage);

            // Spawns in Explosion
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject); // Destroy bullet after hitting an enemy
        }
    }

    // Calculates damage based on level
    private float CalculateDamage()
    {
        float level = ExperienceManager.Instance.GetCurrentLevel();

        // Damage increased by each level
        damage += level;

        return damage;
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
