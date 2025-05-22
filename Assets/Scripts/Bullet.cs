using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public float speed = 8f;
    public float range = 25;
    public float distanceTravelled = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        Vector3 forwardVector = transform.up;
        transform.position = transform.position + forwardVector * speed * dt;

        distanceTravelled += speed * dt;
        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }

    }
}
