using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicProjectile : MonoBehaviour
{
    private float poisonDamage;
    private float poisonInterval;
    private float poisonDuration;
    public float distanceTravelled = 0;
    public float speed = 8f;
    public float range = 25;

    public void SetPoison(float damage, float interval, float duration)
    {
        poisonDamage = damage;
        poisonInterval = interval;
        poisonDuration = duration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player_Movement player = collision.GetComponent<Player_Movement>();
        if (player != null)
        {
            PoisonEffect effect = player.GetComponent<PoisonEffect>();
            if (effect == null)
            {
                effect = player.gameObject.AddComponent<PoisonEffect>();
            }
            effect.Initialize(poisonDamage, poisonInterval, poisonDuration);
            Destroy(gameObject);
        }
        // Optionally, destroy on hitting anything else
    }
    void Update()
    {
        //bullet moving forward
        float dt = Time.deltaTime;
        Vector3 forwardVector = transform.up;
        transform.position = transform.position + forwardVector * speed * dt;

        distanceTravelled += speed * dt;

        //destroying bullet after a range
        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }
    }
}