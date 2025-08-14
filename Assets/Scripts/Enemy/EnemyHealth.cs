using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float baseMaxHealth = 100f; // Base health for the enemy  
    private float currentHealth;
    private Zombie_Following zombie;

    private void Awake()
    {
        zombie = GetComponent<Zombie_Following>();
    }

    public void RoundsHealthMultiplier(int round)
    {
        float multiplier = 1f + (round - 1) * 0.35f; // Increase health by 35% each round  
        currentHealth = baseMaxHealth * multiplier;

        Debug.Log($"{name} initialized with {currentHealth} HP for round {round}");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} took {damage} damage, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log($"{name} has died.");
            zombie?.Die(); // Let Zombie_Following handle the death event + cleanup
        }
    }
}

