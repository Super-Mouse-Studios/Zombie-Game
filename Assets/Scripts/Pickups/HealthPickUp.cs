using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("medkit");
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Movement move = collision.GetComponent<Player_Movement>(); 
            if (move != null)
            {
                SoundManager.Instance.PlaySound("Drink");

                move.Healing();
                Debug.Log("Healing");
                Destroy(gameObject);
            }
        }
    }
}
