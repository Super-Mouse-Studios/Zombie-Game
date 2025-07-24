using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shopPanel;
    public Button continueButton;

    public int playerCurrency = 100; // Default for testing
    public TMP_Text currencyText; 
    public TMP_Text purchaseMessageText; // drag the TMP in Inspector
    public float messageDuration = 2f;   // how long to show the message

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
                ShowPurchaseMessage($"Purchased {weaponName} for {price}!");
                Debug.Log($"Purchased {weaponName} for {price} currency.");
            }
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
    
    public void ShowPurchaseMessage(string message)
    {
        StopAllCoroutines(); // stops old fades if clicking fast
        StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        purchaseMessageText.text = message;

        // Fade in
        Color c = purchaseMessageText.color;
        c.a = 1f;
        purchaseMessageText.color = c;

        yield return new WaitForSeconds(messageDuration);

        // Fade out
        c.a = 0f;
        purchaseMessageText.color = c;

        purchaseMessageText.text = "";
    }

}