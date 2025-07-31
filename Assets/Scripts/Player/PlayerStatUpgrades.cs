using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatUpgrades : MonoBehaviour
{
    public static PlayerStatUpgrades Instance;

    // Movement and Dodge upgrades
    public float maxHPUpgrade = 0f;
    public float movementSpeedUpgrade = 0f;
    public float dodgeSpeedUpgrade = 0f;
    public float dodgeCooldownReduction = 0f;

    // Shooting upgrades
    public float fireRateIncrease = 0f;
    public float critRateIncrease = 0f;
    public float critDamageIncrease = 0f;
    public float meleeDamageIncrease = 0f; // if you want to affect melee attack speed/damage
    public float bulletDamageIncrease = 0f;
    public int maxAmmoUpgrade = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}