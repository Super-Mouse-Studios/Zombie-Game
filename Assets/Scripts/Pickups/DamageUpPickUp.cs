using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpPickUp : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("battery");
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting shooting = collision.GetComponent<Shooting>(); // Get the Shooting component from the Player
            if (shooting != null)
            {
                SoundManager.Instance.PlaySound("DamageUp");

                shooting.DamageUp();
                Debug.Log("Damage Up");
                Destroy(gameObject);
            }
        }
    }
}
