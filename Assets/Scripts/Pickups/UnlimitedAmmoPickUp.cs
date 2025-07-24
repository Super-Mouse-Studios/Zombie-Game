using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlimitedAmmoPickUp : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("ammo-Sniper");
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting shooting = collision.GetComponent<Shooting>();
            if (shooting != null)
            {
                SoundManager.Instance.PlaySound("Drink");

                shooting.UnlimitedAmmo();
                Debug.Log("Unlimited Ammo");
                Destroy(gameObject);
            }
        }
    }
}
