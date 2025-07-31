using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmmoShopButton : MonoBehaviour
{
    [Header("Ammo Data")]
    public int ammoAmount = 10;
    public int ammoPrice = 20;

    [Header("UI Elements")]
    public TextMeshProUGUI ammoInfoText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClickBuyAmmo);
        else
            Debug.LogWarning("Button component missing from AmmoShopButton GameObject.");
    }

    private void Start()
    {
        if (ammoInfoText != null)
            ammoInfoText.text = $"+{ammoAmount} Ammo for ${ammoPrice}";
        else
            Debug.LogWarning("Ammo info text not assigned in inspector.");
    }

    private void OnClickBuyAmmo()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.BuyAmmo(ammoAmount, ammoPrice);
        }
        else
        {
            Debug.LogWarning("ShopManager instance not found.");
        }
    }
}