using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    
    public void updateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
    
}
