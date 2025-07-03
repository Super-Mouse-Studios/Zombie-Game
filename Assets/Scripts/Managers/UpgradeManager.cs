using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [System.Serializable]
    public class Upgrade
    {
        public string upgradeName;
        public string description;
        public Sprite icon;
        public UpgradeType type;
      
    }

    public enum UpgradeType
    {
        Health,
        Speed,
        Damage,
        DodgeCooldown,
        FireRate,
    }

    public List<Upgrade> allUpgrades;

    public GameObject upgradePanel; // Panel to display upgrades
    public Button[] upgradeButtons; // Buttons for each upgrade
    public TextMeshProUGUI[] upgradeTitles; // Text for each upgrade
    public TextMeshProUGUI[] upgradeDescriptions; // Text for each upgrade description
    public Button continueButton; // Button to continue after upgrades to next round

    private List<Upgrade> chosenUpgrades = new List<Upgrade>();
    public static UpgradeManager Instance;

    private void Awake()
    {
       Instance = this;
        upgradePanel.SetActive(false); // Hide upgrade panel at start
    }

    public void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true); // Show the upgrade panel
        Time.timeScale = 0f; // Pause the game

        chosenUpgrades.Clear();
        List<Upgrade> pool = new List<Upgrade>(allUpgrades);

        for (int i = 0; i < 3; i++) 
        { 
        if (pool.Count == 0) break; // If no upgrades left, exit loop
            int index = Random.Range(0, pool.Count);
            Upgrade selected = pool[index];
            chosenUpgrades.Add(selected);
            pool.RemoveAt(index); // Remove selected upgrade from pool

            upgradeTitles[i].text = selected.upgradeName; // Set upgrade title
            upgradeDescriptions[i].text = selected.description; // Set upgrade description

            int capturedIndex = i; // Capture index for button click event
            upgradeButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners
            //upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(chosenUpgrades[capturedIndex])); // Add listener for button click
        }

        continueButton.onClick.RemoveAllListeners(); // Clear previous listeners
        continueButton.onClick.AddListener(HideUpgrades); // Add listener for continue button

    }

    //public void ApplyUpgrade(Upgrade upgrade)
    //{
    //    switch (upgrade.type)
    //    {
    //        case UpgradeType.Health:
    //            PlayerStats.Instance.IncreaseMaxHP(); // Increase player health
    //            break;
    //        case UpgradeType.Speed:
    //            PlayerStats.Instance.IncreaseSpeed(); // Increase player speed
    //            break;
    //        case UpgradeType.Damage:
    //            PlayerStats.Instance.IncreaseDamage(); // Increase player damage
    //            break;
    //        case UpgradeType.DodgeCooldown:
    //            PlayerStats.Instance.ReduceDodgeCooldown(); // Reduce dodge cooldown
    //            break;
    //        case UpgradeType.FireRate:
    //            PlayerStats.Instance.IncreaseFireRate(); // Increase fire rate
    //            break;
    //    }
    //}

    public void HideUpgrades()
    {
        upgradePanel.SetActive(false); // Hide the upgrade panel
        Time.timeScale = 1f; // Resume the game

        Rounds roundManager = FindObjectOfType<Rounds>();
        if (roundManager != null)
        {
            roundManager.StartRound(); // Start the next round
        }
    }
}

