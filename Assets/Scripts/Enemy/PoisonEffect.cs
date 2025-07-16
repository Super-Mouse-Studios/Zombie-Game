using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private float tickDamage;
    private float tickInterval;
    private float duration;
    private float timer;
    private float tickTimer;

    public void Initialize(float damage, float interval, float totalDuration)
    {
        tickDamage = damage;
        tickInterval = interval;
        duration = totalDuration;
        timer = 0f;
        tickTimer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            Player_Movement player = GetComponent<Player_Movement>();
            if (player != null)
                player.PlayerTakeDamage(tickDamage);
            tickTimer = 0f;
        }
        if (timer >= duration)
        {
            Destroy(this);
        }
    }
}