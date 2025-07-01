using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            zombieComponent.EnemyTakeDamage(CalculateDamage());
            Debug.Log($"{collision.name} took { CalculateDamage() } damage ({ExperienceManager.Instance.GetCurrentLevel()} from levels)");
            
            //destroying bullet after hitting
            Destroy(gameObject);
        }
        

    }

    // Calculates damage based on level
    private int CalculateDamage()
    {
        int damage = 5; // Base damage
        int level = ExperienceManager.Instance.GetCurrentLevel();

        // Damage increased by each level
        damage += level;

        return damage;
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
