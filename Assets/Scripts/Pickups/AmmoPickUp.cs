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
                if (Random.Range(1, 15) == 1) // 7.5% chance to get 7x amount
                    ammo *= 7;
                else if (Random.Range(1, 5) == 1) // 20% for 2x
                    ammo *= 2;
                shooting.IncreaseAmmo(ammo);
                Debug.Log($"Gained {ammo} ammo from pickup");
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
