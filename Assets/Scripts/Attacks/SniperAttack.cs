using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAttack : MonoBehaviour
{
    public float speed = 26f;
    public float range = 32;
    [SerializeField] float distanceTravelled = 0;
    public float damage = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //bullet moving forward
        float dt = Time.deltaTime;
        Vector3 forwardVector = transform.up;
        transform.position = transform.position + forwardVector * speed * dt;

        distanceTravelled += speed * dt;
        if (transform.localScale.y <= 16) // Bullet Scales in size as it travels
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + speed * dt, transform.localScale.z); 

        //destroying bullet after a range
        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }
        speed += dt;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Calculate damage based on level
            damage = CalculateDamage();
            enemyHealth.TakeDamage(damage);
        }
    }
    
    // Calculates damage based on level
    private int CalculateDamage()
    {
        int damage = 12; // Base damage
        int level = ExperienceManager.Instance.GetCurrentLevel();

        // Damage increased by each level
        damage += level;

        return damage;
    }
}
