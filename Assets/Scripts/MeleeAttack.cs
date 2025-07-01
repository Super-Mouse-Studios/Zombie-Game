using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, .370556f); // Destroyes GameObject after animation plays
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //checking if collision have enemy component on it
        if (collision.gameObject.TryGetComponent<Zombie_Following>(out Zombie_Following zombieComponent))
        {
            //zombie taking damage
            zombieComponent.EnemyTakeDamage(CalculateDamage());
            Debug.Log($"{collision.name} took {CalculateDamage()} damage ({ExperienceManager.Instance.GetCurrentLevel()} from levels)");
        }
    }
    
    // Calculates damage based on level
    private int CalculateDamage()
    {
        int damage = 10; // Base damage
        int level = ExperienceManager.Instance.GetCurrentLevel();

        // Damage increased by each level
        damage += level;

        return damage;
    }
}
