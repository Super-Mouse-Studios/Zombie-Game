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