using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("ammo");
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting shooting = collision.GetComponent<Shooting>(); // Get the Shooting component from the Player
            if (shooting != null)
            {
                SoundManager.Instance.PlaySound("Reload");

                int ammo = Random.Range(4, 7); // Randomly Generate ammo gain from pickup
                if (Random.Range(1, 20) == 1) // 5% chance to get 5x amount
                    ammo *= 5;
                else if (Random.Range(1, 8) == 1) // 12.5% for 2x
                    ammo *= 2;
                shooting.IncreaseAmmo(ammo);
                Debug.Log($"Gained {ammo} ammo from pickup");
                Destroy(gameObject);
            }
        }
    }
}
