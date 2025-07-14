using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlaySound("Explosion");
        Destroy(gameObject, 1f); // Destroys game object after explosion is played
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // checking if collision have enemy component on it
        if (collision.gameObject.TryGetComponent<Zombie_Following>(out Zombie_Following zombieComponent))
        {
            //zombie taking damage
            zombieComponent.EnemyTakeDamage(CalculateDamage());
            Debug.Log($"{collision.name} took {CalculateDamage()} damage ({ExperienceManager.Instance.GetCurrentLevel()} from levels)");
        }
        else if (collision.gameObject.TryGetComponent<Player_Movement>(out Player_Movement playerComponent)) // Checks if collision has player script
        {
            playerComponent.PlayerTakeDamage(2); // Player takes 2 damage from explosion
            Debug.Log($"{collision.name} took {2} damage ({ExperienceManager.Instance.GetCurrentLevel()} from levels)");
        }
    }

    private float CalculateDamage()
    {
        float damage = 15; // Base damage of explosion
        float level = ExperienceManager.Instance.GetCurrentLevel();

        // Damage increased by each level
        damage += level;

        return damage;
    } 
}
