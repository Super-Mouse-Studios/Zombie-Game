using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyAmmoButton : MonoBehaviour
{
    public int ammoPrice = 20;
    public int ammoAmount = 10;

    [SerializeField] private TextMeshProUGUI ammoInfoText;

    private void Start()
    {
        if (ammoInfoText != null)
        {
            ammoInfoText.text = $"+{ammoAmount} Ammo for ${ammoPrice}";
        }
    }

    public void OnClickBuyAmmo()
    {
        ShopManager.Instance.BuyAmmo(ammoAmount, ammoPrice);
    }
}