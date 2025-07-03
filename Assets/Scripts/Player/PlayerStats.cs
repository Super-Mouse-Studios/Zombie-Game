using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
 public static PlayerStats Instance;

    public float maxHealth = 100f;
    public float currentHealth;
    public float damage = 10f;
    public float speed = 5f;
    public float dodgeCooldown = 1f; // Assuming you have a dodge cooldown variable
    public float fireRate = 1f; // Assuming you have a fire rate variable

    public event Action<float> OnHealthChanged; // Event to notify health changes
    public event Action<float> OnDamageChanged; // Event to notify damage changes
    public event Action<float> OnSpeedChanged; // Event to notify speed changes 
    public event Action<float> OnDodgeCooldownChanged; // Event to notify dodge cooldown changes    
    public event Action<float> OnFireRateChanged; // Event to notify fire rate changes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to max health

    }

    public void IncreaseMaxHP(float amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // Reset current health to new max
        Debug.Log("Max Health increased to: " + maxHealth);
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;
        Debug.Log("Damage increased to: " + damage);
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log("Speed increased to: " + speed);
    }

    public void ReduceDodgeCooldown(float amount)
    {
        // Assuming you have a dodge cooldown variable, reduce it
        dodgeCooldown -= amount; 
        Debug.Log("Dodge cooldown reduced by: " + amount);
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate += amount;
        Debug.Log("Fire rate increased to: " + fireRate);
    }
}
