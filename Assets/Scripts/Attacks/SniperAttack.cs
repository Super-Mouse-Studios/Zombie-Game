using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAttack : MonoBehaviour
{
    public float speed = 26f;
    public float range = 32;
    [SerializeField] float distanceTravelled = 0;
    public float baseDamage = 5f;
    Shooting shooting;

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
            float damage = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shooting>().CalculateDamage(baseDamage, collision.transform.position);
            enemyHealth.TakeDamage(damage);
        }
    }
}
