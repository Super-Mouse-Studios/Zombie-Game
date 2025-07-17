using UnityEngine;
using UnityEngine.UI;

public class WeaponShopButton : MonoBehaviour
{
    public Shooting.ShootingBehavours weaponType;  // The weapon this button represents
    private Button button;
    public int price;                              // Price shows in Inspector now
    public string weaponName;                      // Optional: weapon name to display

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void BuyWeapon()
    {
        ShopManager.Instance.BuyWeapon(weaponType, price, weaponName);
    }

    private void OnClick()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.BuyWeapon(weaponType, price, weaponName);
        }
        else
        {
            Debug.LogWarning("ShopManager instance not found.");
        }
    }
}