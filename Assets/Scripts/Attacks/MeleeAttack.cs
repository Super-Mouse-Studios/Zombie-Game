using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    float baseDamage = 10; // Base damage
    [SerializeField] Shooting shooting;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, .370556f); // Destroyes GameObject after animation plays
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //checking if collision have enemy component on it
        if (collision.gameObject.TryGetComponent<Zombie_Following>(out Zombie_Following zombieComponent))
        {
            float damage = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shooting>().CalculateDamage(baseDamage);
            //zombie taking damage
            zombieComponent.EnemyTakeDamage(damage);
            Debug.Log($"{collision.name} took {damage} damage ({ExperienceManager.Instance.GetCurrentLevel()} from levels)");
        }
    }
}
