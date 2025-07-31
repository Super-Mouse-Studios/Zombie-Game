using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritPickUp : MonoBehaviour
{
    Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        an.Play("tools");
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting shooting = collision.GetComponent<Shooting>(); 
            if (shooting != null)
            {
                SoundManager.Instance.PlaySound("DoubleCrit");

                shooting.DoubleCritRate();
                Debug.Log("Doubling Crit Rate");
                Destroy(gameObject);
            }
        }
    }
}
