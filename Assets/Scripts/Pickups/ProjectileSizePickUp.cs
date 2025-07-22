using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSizePickUp : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("bluePrint");
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting shooting = collision.GetComponent<Shooting>(); // Get the Shooting component from the Player
            if (shooting != null)
            {
                SoundManager.Instance.PlaySound("AttackSize");

                shooting.AttackSizeTimer();
                Debug.Log("Attack Size Increased");
                Destroy(gameObject);
            }
        }
    }
}
