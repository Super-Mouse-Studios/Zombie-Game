using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI EnemyKilledText;
    // List<Zombie_Following> zombieList = new List<Zombie_Following>();
   
    public static GameManager instance;

    private int zombiesDead = 0;
    
    void Awake()
    {
        instance = this;
        zombiesDead = 0;
        // zombieList = FindObjectsOfType<Zombie_Following>().ToList();
        UpdateEnemyKilledText();
    }

    public void ZombieDied()
    {
        zombiesDead++;
        UpdateEnemyKilledText();
    }
    
    void UpdateEnemyKilledText()
    {
        EnemyKilledText.text = $"Zombies left: {zombiesDead}";
    }

    
}
