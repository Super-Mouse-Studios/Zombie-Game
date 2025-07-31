using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI Elements")]
    public GameObject shopPanel;
    public Button continueButton;
    public TMP_Text currencyText;
    public TMP_Text purchaseMessageText;

    [Header("Settings")]
    public int playerCurrency = 100; // Starting currency for testing
    public float messageDuration = 2f; // Duration to show purchase messages

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        shopPanel.SetActive(false);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        UpdateCurrencyDisplay();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
        UpdateCurrencyDisplay();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BuyWeapon(Shooting.ShootingBehavours weaponType, int price, string weaponName)
    {
        if (playerCurrency >= price)
        {
            playerCurrency -= price;
            UpdateCurrencyDisplay();

            var shooting = FindObjectOfType<Shooting>();
            if (shooting != null)
            {
                shooting.shootMode = weaponType;
                shooting.currentlyHeld = weaponType;
                ShowPurchaseMessage($"Purchased {weaponName} for ${price}!");
                Debug.Log($"Purchased {weaponName} for {price} currency.");
            }
        }
        else
        {
            ShowPurchaseMessage("Not enough currency!");
        }
    }

    public void BuyAmmo(int amount, int cost)
    {
        if (playerCurrency >= cost)
        {
            playerCurrency -= cost;
            UpdateCurrencyDisplay();

            Shooting shooting = FindObjectOfType<Shooting>();
            if (shooting != null)
            {
                shooting.IncreaseAmmo(amount);
                ShowPurchaseMessage($"Bought +{amount} Ammo for ${cost}!");
                Debug.Log($"Bought +{amount} Ammo for {cost} currency.");
            }
            else
            {
                Debug.LogWarning("No Shooting script found on player.");
            }
        }
        else
        {
            ShowPurchaseMessage("Not enough currency to buy ammo.");
        }
    }

    public void BuyUpgrade(UpgradeShopButton.UpgradeData upgrade)
    {
        if (playerCurrency >= upgrade.price)
        {
            playerCurrency -= upgrade.price;
            UpdateCurrencyDisplay();

            var upgrades = PlayerStatUpgrades.Instance;

            switch (upgrade.upgradeType)
            {
                case UpgradeShopButton.UpgradeType.MaxHP:
                    upgrades.maxHPUpgrade += upgrade.upgradeAmount;
                    Debug.Log($"Max HP increased by {upgrade.upgradeAmount}. New total: {upgrades.maxHPUpgrade}");
                    break;
                case UpgradeShopButton.UpgradeType.MovementSpeed:
                    upgrades.movementSpeedUpgrade += upgrade.upgradeAmount;
                    Debug.Log($"Movement Speed increased by {upgrade.upgradeAmount}. New total: {upgrades.movementSpeedUpgrade}");
                    break;
                case UpgradeShopButton.UpgradeType.DodgeSpeed:
                    upgrades.dodgeSpeedUpgrade += upgrade.upgradeAmount;
                    Debug.Log($"Dodge Speed increased by {upgrade.upgradeAmount}. New total: {upgrades.dodgeSpeedUpgrade}");
                    break;
                case UpgradeShopButton.UpgradeType.DodgeCooldownReduction:
                    upgrades.dodgeCooldownReduction += upgrade.upgradeAmount;
                    Debug.Log($"Dodge Cooldown Reduction increased by {upgrade.upgradeAmount}. New total: {upgrades.dodgeCooldownReduction}");
                    break;
                case UpgradeShopButton.UpgradeType.FireRate:
                    upgrades.fireRateIncrease += upgrade.upgradeAmount;
                    Debug.Log($"Fire Rate increased by {upgrade.upgradeAmount}. New total: {upgrades.fireRateIncrease}");
                    break;
                case UpgradeShopButton.UpgradeType.CritRate:
                    upgrades.critRateIncrease += upgrade.upgradeAmount;
                    Debug.Log($"Crit Rate increased by {upgrade.upgradeAmount}. New total: {upgrades.critRateIncrease}");
                    break;
                case UpgradeShopButton.UpgradeType.CritDamage:
                    upgrades.critDamageIncrease += upgrade.upgradeAmount;
                    Debug.Log($"Crit Damage increased by {upgrade.upgradeAmount}. New total: {upgrades.critDamageIncrease}");
                    break;
                case UpgradeShopButton.UpgradeType.MeleeDamage:
                    upgrades.meleeDamageIncrease += upgrade.upgradeAmount;
                    Debug.Log($"Melee Damage increased by {upgrade.upgradeAmount}. New total: {upgrades.meleeDamageIncrease}");
                    break;
                case UpgradeShopButton.UpgradeType.BulletDamage:
                    upgrades.bulletDamageIncrease += upgrade.upgradeAmount;
                    Debug.Log($"Bullet Damage increased by {upgrade.upgradeAmount}. New total: {upgrades.bulletDamageIncrease}");
                    break;
                case UpgradeShopButton.UpgradeType.MaxAmmo:
                    int intAmount = Mathf.RoundToInt(upgrade.upgradeAmount);
                    upgrades.maxAmmoUpgrade += intAmount;
                    Debug.Log($"Max Ammo increased by {intAmount}. New total: {upgrades.maxAmmoUpgrade}");
                    break;
                default:
                    Debug.LogWarning("Unhandled upgrade type: " + upgrade.upgradeType);
                    break;
            }

            ShowPurchaseMessage($"Purchased {upgrade.upgradeName} for ${upgrade.price}!");
            Debug.Log($"Purchased upgrade {upgrade.upgradeName} for {upgrade.price} currency.");
        }
        else
        {
            ShowPurchaseMessage("Not enough currency for upgrade!");
            Debug.Log("Not enough currency to buy upgrade.");
        }
    }

    private void OnContinue()
    {
        CloseShop();
        Rounds.Instance?.ContinueToNextRound();
    }

    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        UpdateCurrencyDisplay();
        Debug.Log($"Currency added: {amount}, total: {playerCurrency}");
    }

    public void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + playerCurrency;
        }
    }

    public void ShowPurchaseMessage(string message)
    {
        if (purchaseMessageText == null)
        {
            Debug.LogWarning("Purchase message text is not assigned!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        purchaseMessageText.text = message;

        Color color = purchaseMessageText.color;
        color.a = 1f;
        purchaseMessageText.color = color;

        yield return new WaitForSeconds(messageDuration);

        color.a = 0f;
        purchaseMessageText.color = color;

        purchaseMessageText.text = "";
    }
}