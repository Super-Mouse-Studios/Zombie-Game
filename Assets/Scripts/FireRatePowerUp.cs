using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRatePowerUp : MonoBehaviour
{
    public float fireRateMult = 2f; // Amount to increase fire rate by
    public float duration = 5f; // Duration of the power-up effect in seconds

    void Start()
    {
        Destroy(gameObject, 10f);  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting shooting = collision.GetComponent<Shooting>(); // Get the Shooting component from the Player
            if (shooting != null)
            {
                SoundManager.Instance.PlaySound("FireRate"); 
                StartCoroutine(ApplyFireRatePowerUp(shooting)); // Pass the Shooting component to the coroutine
                GetComponent<SpriteRenderer>().enabled = false; // Disable the sprite renderer to hide the power-up
                GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent multiple triggers
            }
        }
    }

    private IEnumerator ApplyFireRatePowerUp(Shooting shooting)
    {
        float originalFireRate = shooting.fireRate;

        shooting.fireRate *= fireRateMult; // Increase the fire rate
        Debug.Log("Fire rate increased to: " + shooting.fireRate);

        yield return new WaitForSeconds(duration); // Wait for the duration of the power-up

        shooting.fireRate = originalFireRate; // Reset the fire rate to its original value
        Debug.Log("Fire rate reset to: " + shooting.fireRate);

        Destroy(gameObject); // Destroy the power-up object after use
    }
}
