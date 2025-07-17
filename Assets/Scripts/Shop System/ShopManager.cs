using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shopPanel;
    public Button continueButton;

    public int playerCurrency = 100; // Default for testing
    public Text currencyText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        shopPanel.SetActive(false);
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
                Debug.Log($"Purchased {weaponName} for {price} currency.");
            }
            else
            {
                Debug.LogWarning("Shooting component not found.");
            }
        }
        else
        {
            Debug.Log($"Not enough currency to buy {weaponName}! You have {playerCurrency}, need {price}.");
        }
    }

    private void OnContinue()
    {
        CloseShop();
        Rounds.Instance?.ContinueToNextRound();
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + playerCurrency;
        }
    }
}