using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

//****************************************
//**this script is for player health bar**
//****************************************

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    
    public void updateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
    
}
