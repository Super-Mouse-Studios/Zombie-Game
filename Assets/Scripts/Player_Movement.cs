using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    private Rigidbody2D rb; 
    public float speed = 5f; // Speed of the player
    public Vector3 inputvector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
