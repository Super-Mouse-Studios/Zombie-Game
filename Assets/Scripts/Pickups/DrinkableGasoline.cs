using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkableGasoline : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("gas");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Movement move = collision.GetComponent<Player_Movement>(); 
            if (move != null)
            {
                SoundManager.Instance.PlaySound("Drink");

                move.Gasoline();
                Debug.Log("Drinkable Gasoline");
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
