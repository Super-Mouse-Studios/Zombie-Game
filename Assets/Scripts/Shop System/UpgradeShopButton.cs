using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeShopButton : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeData
    {
        public string upgradeName;
        public int price;
        public UpgradeType upgradeType;
        public float upgradeAmount;
    }

    public enum UpgradeType
    {
        MaxHP,
        MovementSpeed,
        DodgeSpeed,
        DodgeCooldownReduction,
        FireRate,
        CritRate,
        CritDamage,
        MeleeDamage,
        BulletDamage,
        MaxAmmo
    }

    [Header("Available Upgrades")]
    public UpgradeData[] availableUpgrades;

    [Header("UI Elements")]
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI priceText;

    private Button button;
    private UpgradeData selectedUpgrade;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClickBuyUpgrade);
        else
            Debug.LogWarning("Button component missing from UpgradeShopButton GameObject.");
    }

    private void OnEnable()
    {
        RandomizeUpgrade();
    }

    private void RandomizeUpgrade()
    {
        if (availableUpgrades != null && availableUpgrades.Length > 0)
        {
            int index = Random.Range(0, availableUpgrades.Length);
            selectedUpgrade = availableUpgrades[index];

            if (upgradeNameText != null)
                upgradeNameText.text = selectedUpgrade.upgradeName;

            if (priceText != null)
                priceText.text = "$" + selectedUpgrade.price;
        }
        else
        {
            Debug.LogWarning("No upgrade data available to randomize.");
        }
    }

    private void OnClickBuyUpgrade()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogWarning("ShopManager instance not found.");
            return;
        }

        ShopManager.Instance.BuyUpgrade(selectedUpgrade);
    }
}