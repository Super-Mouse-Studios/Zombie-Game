using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosion : MonoBehaviour
{
    float baseDamage = 15; // Base damage of explosion
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlaySound("Explosion");
        Destroy(gameObject, 1f); // Destroys game object after explosion is played
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // checking if collision have enemy component on it
        if (collision.gameObject.TryGetComponent<Zombie_Following>(out Zombie_Following zombieComponent))
        {
            //zombie taking damage
            float damage = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooting>().CalculateDamage(baseDamage);
            zombieComponent.EnemyTakeDamage(damage);
        }
        else if (collision.gameObject.TryGetComponent<Player_Movement>(out Player_Movement playerComponent)) // Checks if collision has player script
        {
            playerComponent.PlayerTakeDamage(2); // Player takes 2 damage from explosion
        }
    }
}
