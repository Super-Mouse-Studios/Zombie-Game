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
    private Vector2 moveDirection = Vector2.up;

    public void SetPoison(float damage, float interval, float duration)
    {
        poisonDamage = damage;
        poisonInterval = interval;
        poisonDuration = duration;
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
    }

    void Update()
    {
        float dt = Time.deltaTime;
        transform.position += (Vector3)moveDirection * speed * dt;
        distanceTravelled += speed * dt;

        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }
    }
}