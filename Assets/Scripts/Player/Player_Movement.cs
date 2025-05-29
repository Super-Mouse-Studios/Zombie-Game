using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//**************************************
//**this script is for player movement**
//**************************************
public class Player_Movement : MonoBehaviour
{

    //health status
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Healthbar healthbar;
    
    public float speed = 5f; // Speed of the player
    public Vector3 inputvector = Vector3.zero;
    private Rigidbody2D rb; 


    
    void Start()
    {
        //health status at the start of the game
        currentHealth = maxHealth;

        healthbar.updateHealthBar(maxHealth, currentHealth); //updating health bar
    }
    
    //player taking damage and dying
    public void PlayerTakeDamage(float PlayerDamageAmount)
    {
        currentHealth -= PlayerDamageAmount; //10 -> 9 -> 8 -> 7 -> 6 -> 5 -> 4 -> 3 -> 2 -> 1 -> 0
        healthbar.updateHealthBar(maxHealth, currentHealth); //updating health bar

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        inputvector = new Vector3(inputX, inputY, 0);


        if (!(inputvector.x == 0 && inputvector.y == 0))
        {
            float dt = Time.deltaTime;
            Vector3 direction = inputvector.normalized;
            transform.position += inputvector / inputvector.magnitude * speed * dt;
            transform.position += direction * speed * dt;
            Vector3 rotatedDirection = new Vector3(-direction.x, direction.y, 0);
            transform.up = direction;
        }
        
    }
}
