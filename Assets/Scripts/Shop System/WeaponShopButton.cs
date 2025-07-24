using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponShopButton : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text priceText;

    private Button button;
    
    [System.Serializable]
    public class WeaponData
    {
        public string name;
        public int price;
        public Shooting.ShootingBehavours behaviour;
    }


    [Header("Weapon Data")]
    public WeaponData[] allWeapons; // Assign list of weapons in Inspector

    private WeaponData assignedWeapon;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        AssignRandomWeapon();
    }

    private void AssignRandomWeapon()
    {
        if (allWeapons.Length == 0)
        {
            Debug.LogError("No weapons assigned!");
            return;
        }

        // Choose one randomly
        assignedWeapon = allWeapons[Random.Range(0, allWeapons.Length)];

        // Update UI
        if (nameText != null)
            nameText.text = assignedWeapon.name;

        if (priceText != null)
            priceText.text = "$" + assignedWeapon.price;
    }

    private void OnClick()
    {
        if (ShopManager.Instance != null && assignedWeapon != null)
        {
            ShopManager.Instance.BuyWeapon(assignedWeapon.behaviour, assignedWeapon.price, assignedWeapon.name);
        }
    }
}