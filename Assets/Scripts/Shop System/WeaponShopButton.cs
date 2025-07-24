using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponShopButton : MonoBehaviour
{
    [Header("Weapon Data List")]
    public List<WeaponData> availableWeaponsData; // Assign list of weapons in Inspector

    [Header("UI Elements")]
    public TextMeshProUGUI priceText;       // Assign TMP text for price
    public TextMeshProUGUI weaponNameText;  // Assign TMP text for name

    private Button button;
    private WeaponData selectedWeaponData;

    [System.Serializable]
    public class WeaponData
    {
        public Shooting.ShootingBehavours weaponType;
        public int price;
        public string weaponName;
    }

    private void OnEnable()
    {
        RandomizeWeapon();
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);
        else
            Debug.LogWarning("Button component missing from WeaponShopButton GameObject.");
    }

    public void RandomizeWeapon()
    {
        if (availableWeaponsData != null && availableWeaponsData.Count > 0)
        {
            int index = Random.Range(0, availableWeaponsData.Count);
            selectedWeaponData = availableWeaponsData[index];

            if (weaponNameText != null)
                weaponNameText.text = selectedWeaponData.weaponName;
            else
                Debug.LogWarning("Weapon name text not assigned in inspector.");

            if (priceText != null)
                priceText.text = "$" + selectedWeaponData.price;
            else
                Debug.LogWarning("Price text not assigned in inspector.");
        }
        else
        {
            Debug.LogWarning("No weapon data available to randomize.");
        }
    }

    public void BuyWeapon()
    {
        if (selectedWeaponData != null && ShopManager.Instance != null)
        {
            ShopManager.Instance.BuyWeapon(
                selectedWeaponData.weaponType,
                selectedWeaponData.price,
                selectedWeaponData.weaponName
            );
        }
        else
        {
            Debug.LogWarning("ShopManager not found or weapon data is null.");
        }
    }

    private void OnClick()
    {
        BuyWeapon();
    }
}