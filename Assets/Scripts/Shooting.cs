using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class Shooting : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    public GameObject projectilePrefab;

    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject meleePrefab;
    [SerializeField] GameObject sniperShotPrefab;
    [SerializeField] GameObject shotgunShotPrefab;

    [Header("Shooting")]
    public bool isTriggerDown;
    public float timeUntilReloaded, meleeCooldown = 0;

    // Base stats (will be modified by upgrades)
    public float baseFireRate = 1f; // shots per second
    public float baseMeleeRate = 1f; // attacks per second

    public ShootingBehavours shootMode = ShootingBehavours.Basic;
    public ShootingBehavours currentlyHeld = ShootingBehavours.Basic;

    private ShootingAnimation sa;
    [SerializeField] SpriteRenderer sr;

    // Base crit stats (modified by upgrades)
    [SerializeField][Range(0f, 1f)] float baseCritRate = 0.15f;
    [SerializeField] float baseCritDamage = 2f;

    [Header("Pickups")]
    private float attackSizeLength = 0;
    [SerializeField][Range(1.25f, 3f)] float attackSizeMultiplier = 1.5f; // for powerup
    public float damagePowerUp = 0f;
    [SerializeField][Range(1f, 2f)] float damageMuliplier = 1.5f;
    [SerializeField] float unlimitedAmmoTimer = 0f;
    [SerializeField] float doubleCritTimer = 0f;
    [SerializeField] GameObject textPrefab;

    [Header("Ammo")]
    public int baseMaxAmmo = 500; // base max ammo before upgrades
    private int currentAmmo;
    public TMP_Text Ammo_Display;

    private UnityEngine.Camera mainCam;
    private Vector3 mousePos;
    private Player_Movement pm;

    public enum ShootingBehavours
    {
        Basic,
        Spread,
        Rocket,
        AR,
        Sniper,
        Revolver
    }

    private Transform targetEnemy;

    void Start()
    {
        mainCam = UnityEngine.Camera.main;

        // Set max ammo including upgrades
        currentAmmo = Mathf.Min(30, baseMaxAmmo + PlayerStatUpgrades.Instance?.maxAmmoUpgrade ?? 0);

        sa = GetComponentInChildren<ShootingAnimation>();
        sr = GetComponentInChildren<SpriteRenderer>();
        pm = GetComponentInParent<Player_Movement>();
        sa.PlayAnimation("flicker");
    }

    void Update()
    {
        // DEBUG: Press keys to increase upgrades on the fly (for testing only)
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerStatUpgrades.Instance.maxAmmoUpgrade += 10;
            Debug.Log($"Max Ammo Upgrade increased: {PlayerStatUpgrades.Instance.maxAmmoUpgrade}");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerStatUpgrades.Instance.fireRateIncrease += 0.2f;
            Debug.Log($"Fire Rate Upgrade increased: {PlayerStatUpgrades.Instance.fireRateIncrease}");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerStatUpgrades.Instance.critRateIncrease += 0.05f;
            Debug.Log($"Crit Rate Upgrade increased: {PlayerStatUpgrades.Instance.critRateIncrease}");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerStatUpgrades.Instance.critDamageIncrease += 0.5f;
            Debug.Log($"Crit Damage Upgrade increased: {PlayerStatUpgrades.Instance.critDamageIncrease}");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerStatUpgrades.Instance.bulletDamageIncrease += 2f;
            Debug.Log("Bullet damage increased by 2! Current bonus: " + PlayerStatUpgrades.Instance.bulletDamageIncrease);
        }

        // Update max ammo dynamically with upgrades
        int upgradedMaxAmmo = baseMaxAmmo + (PlayerStatUpgrades.Instance?.maxAmmoUpgrade ?? 0);

        // Clamp current ammo to max ammo (in case upgrades changed)
        currentAmmo = Mathf.Clamp(currentAmmo, 0, upgradedMaxAmmo);

        Ammo_Display.text = $"Ammo left: {(unlimitedAmmoTimer > 0 ? "∞" : currentAmmo.ToString())}";

        isTriggerDown = Input.GetMouseButton(0) || Input.GetButton("Jump");

        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButton(1))
            MeleeAttackBehaviour();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shootMode = ShootingBehavours.Basic;
            sa.PlayAnimation("flicker");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapToSecondary();

        AimTowardsMouse();

        // Calculate upgraded fire rate and melee rate based on upgrades
        float upgradedFireRate = baseFireRate + (PlayerStatUpgrades.Instance?.fireRateIncrease ?? 0f);
        float upgradedMeleeRate = baseMeleeRate + (PlayerStatUpgrades.Instance?.meleeDamageIncrease ?? 0f); // Assuming meleeDamageIncrease can also affect melee attack speed, adjust if needed

        switch (shootMode)
        {
            case ShootingBehavours.Basic:
                if (isTriggerDown)
                    BasicShootingBehaviour(upgradedFireRate);
                break;
            case ShootingBehavours.Spread:
                if (isTriggerDown)
                    SpreadShootingBehaviour(upgradedFireRate);
                break;
            case ShootingBehavours.Rocket:
                if (isTriggerDown)
                    RocketShootingBehaviour(upgradedFireRate);
                break;
            case ShootingBehavours.AR:
                if (isTriggerDown)
                    ARShootingBehaviour(upgradedFireRate);
                break;
            case ShootingBehavours.Sniper:
                if (isTriggerDown)
                    SniperShootingBehaviour(upgradedFireRate);
                break;
            case ShootingBehavours.Revolver:
                if (isTriggerDown)
                    RevolverShootingBehaviour(upgradedFireRate);
                break;
        }

        timeUntilReloaded -= Time.deltaTime;
        if (timeUntilReloaded < 0)
            timeUntilReloaded = 0;

        meleeCooldown -= Time.deltaTime;
        if (meleeCooldown < 0)
            meleeCooldown = 0;

        // Debug weapon swaps (can keep or remove later)
        if (Input.GetKeyDown(KeyCode.Y))
        {
            shootMode = ShootingBehavours.Spread;
            currentlyHeld = ShootingBehavours.Spread;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            shootMode = ShootingBehavours.Rocket;
            currentlyHeld = ShootingBehavours.Rocket;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            shootMode = ShootingBehavours.AR;
            currentlyHeld = ShootingBehavours.AR;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            shootMode = ShootingBehavours.Sniper;
            currentlyHeld = ShootingBehavours.Sniper;
            SwapToSecondary();
        }
        else if (Input.GetKeyDown(KeyCode.RightCommand))
        {
            shootMode = ShootingBehavours.Revolver;
            currentlyHeld = ShootingBehavours.Revolver;
            SwapToSecondary();
        }

        if (attackSizeLength > 0)
            attackSizeLength -= Time.deltaTime;
        if (damagePowerUp > 0)
            damagePowerUp -= Time.deltaTime;
        if (unlimitedAmmoTimer > 0)
            unlimitedAmmoTimer -= Time.deltaTime;
        if (doubleCritTimer > 0)
            doubleCritTimer -= Time.deltaTime;
    }

    void AimTowardsMouse()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ - 91f);

        if (mousePos.x < transform.position.x)
        {
            if (!pm.lookingRight) sr.flipY = false;
            else sr.flipY = true;
        }
        else
        {
            if (!pm.lookingRight) sr.flipY = true;
            else sr.flipY = false;
        }
    }

    void SwapToSecondary()
    {
        switch (currentlyHeld)
        {
            case ShootingBehavours.Spread:
                shootMode = ShootingBehavours.Spread;
                sa.PlayAnimation("flicker 0_3");
                break;
            case ShootingBehavours.Rocket:
                shootMode = ShootingBehavours.Rocket;
                sa.PlayAnimation("flicker 0_5");
                break;
            case ShootingBehavours.AR:
                shootMode = ShootingBehavours.AR;
                sa.PlayAnimation("flicker 0_4");
                break;
            case ShootingBehavours.Sniper:
                shootMode = ShootingBehavours.Sniper;
                sa.PlayAnimation("Flicker");
                break;
            case ShootingBehavours.Revolver:
                shootMode = ShootingBehavours.Revolver;
                sa.PlayAnimation("flicker 0_2");
                break;
            default:
                shootMode = ShootingBehavours.Basic;
                sa.PlayAnimation("flicker");
                break;
        }
    }

    // Updated shooting behaviours to accept fireRate param from upgrades

    void BasicShootingBehaviour(float upgradedFireRate)
    {
        if (timeUntilReloaded <= 0 && currentAmmo > 0)
        {
            sa.PlayShootingAnimation("shot ");
            SoundManager.Instance.PlaySound("Chaingun");

            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            if (attackSizeLength > 0)
                projectile.transform.localScale *= attackSizeMultiplier;

            float secondsPerShot = 1 / upgradedFireRate;
            timeUntilReloaded += secondsPerShot;
            currentAmmo -= (unlimitedAmmoTimer <= 0) ? 1 : 0;
        }
    }

    void SpreadShootingBehaviour(float upgradedFireRate)
    {
        if (timeUntilReloaded <= 0 && currentAmmo >= 3)
        {
            sa.PlayShootingAnimation("shoot 0_3");
            SoundManager.Instance.PlaySound("Shotgun");

            GameObject center = Instantiate(shotgunShotPrefab, transform.position, transform.rotation);
            Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, -12f);
            GameObject left = Instantiate(shotgunShotPrefab, transform.position, leftRotation);
            Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, 12f);
            GameObject right = Instantiate(shotgunShotPrefab, transform.position, rightRotation);

            if (attackSizeLength > 0)
            {
                center.transform.localScale *= attackSizeMultiplier;
                left.transform.localScale *= attackSizeMultiplier;
                right.transform.localScale *= attackSizeMultiplier;
            }

            float secondsPerShot = 1 / upgradedFireRate;
            timeUntilReloaded += secondsPerShot;
            currentAmmo -= (unlimitedAmmoTimer <= 0) ? 3 : 0;
        }
    }

    void RocketShootingBehaviour(float upgradedFireRate)
    {
        if (timeUntilReloaded <= 0 && currentAmmo > 0)
        {
            sa.PlayShootingAnimation("shoot 0_5");
            SoundManager.Instance.PlaySound("Rocket");

            GameObject rocket = Instantiate(rocketPrefab, transform.position, transform.rotation);
            if (attackSizeLength > 0)
                rocket.transform.localScale *= attackSizeMultiplier;

            float secondsPerShot = 1 / (upgradedFireRate / 1.2f);
            timeUntilReloaded += secondsPerShot;
            currentAmmo -= (unlimitedAmmoTimer <= 0) ? 1 : 0;
        }
    }

    void ARShootingBehaviour(float upgradedFireRate)
    {
        if (timeUntilReloaded <= 0 && currentAmmo > 0)
        {
            sa.PlayShootingAnimation("shoot 0_4");
            SoundManager.Instance.PlaySound("AR");

            Instantiate(projectilePrefab, transform.position, transform.rotation);

            float secondsPerShot = 1 / (upgradedFireRate * 4);
            timeUntilReloaded += secondsPerShot;
            currentAmmo -= (unlimitedAmmoTimer <= 0) ? 1 : 0;
        }
    }

    void SniperShootingBehaviour(float upgradedFireRate)
    {
        if (timeUntilReloaded <= 0 && currentAmmo > 0)
        {
            sa.PlayShootingAnimation("Shoot");
            SoundManager.Instance.PlaySound("Charger");

            GameObject projectile = Instantiate(sniperShotPrefab, transform.position, transform.rotation);

            if (attackSizeLength > 0)
                projectile.transform.localScale *= attackSizeMultiplier;

            float secondsPerShot = 1 / (upgradedFireRate / 2);
            timeUntilReloaded += secondsPerShot;
            currentAmmo -= (unlimitedAmmoTimer <= 0) ? 1 : 0;
        }
    }

    // Melee attack updated to respect melee cooldown upgrades and calculate damage
    void MeleeAttackBehaviour()
    {
        float upgradedCritRate = baseCritRate + (PlayerStatUpgrades.Instance?.critRateIncrease ?? 0f);
        float upgradedCritDamage = baseCritDamage + (PlayerStatUpgrades.Instance?.critDamageIncrease ?? 0f);
        float meleeCooldownDuration = 1 / (baseMeleeRate + (PlayerStatUpgrades.Instance?.meleeDamageIncrease ?? 0f)); // Using meleeDamageIncrease to increase attack speed, you can separate this if needed

        if (meleeCooldown <= 0)
        {
            SoundManager.Instance.PlaySound("Knife");

            timeUntilReloaded += 0.25f;

            float meleeOffset = .9f;
            Vector3 spawnPosition = transform.position + transform.up * meleeOffset;

            float meleeAngle = -33f;
            Quaternion meleeRotation = transform.rotation * Quaternion.Euler(0, 0, meleeAngle);

            GameObject meleeObj = Instantiate(meleePrefab, spawnPosition, meleeRotation, this.transform);

            Vector3 parentScale = transform.lossyScale;
            meleeObj.transform.localScale = new Vector3(
                2.9f / parentScale.x,
                2.9f / parentScale.y,
                2.9f / parentScale.z
            );

            if (attackSizeLength > 0)
                meleeObj.transform.localScale *= attackSizeMultiplier;

            meleeCooldown = meleeCooldownDuration;
        }
    }

    public float CalculateDamage(float baseDamage, Vector3 textPosition, float additionalCritRate = 0)
    {
        float level = ExperienceManager.Instance.GetCurrentLevel();
        float damage = baseDamage;

        // Add melee or bullet damage bonus
        if (baseDamage == 10f) // Melee attack default base damage
            damage += PlayerStatUpgrades.Instance?.meleeDamageIncrease ?? 0f;
        else
            damage += PlayerStatUpgrades.Instance?.bulletDamageIncrease ?? 0f;

        damage += level;

        if (damagePowerUp > 0)
            damage *= damageMuliplier;

        // Use upgraded crit values
        float upgradedCritRate = baseCritRate + (PlayerStatUpgrades.Instance?.critRateIncrease ?? 0f);
        float upgradedCritDamage = baseCritDamage + (PlayerStatUpgrades.Instance?.critDamageIncrease ?? 0f);

        if (UnityEngine.Random.value < (doubleCritTimer > 0 ? (upgradedCritRate + additionalCritRate) * 2f : upgradedCritRate + additionalCritRate))
        {
            damage *= upgradedCritDamage;
            SoundManager.Instance.PlaySound("Crit");
            Debug.Log($"CRITICAL HIT! {damage} damage done: base {baseDamage}, level {level}, crit x{upgradedCritDamage}" + (damagePowerUp > 0 ? $", boosted by x{damageMuliplier}" : ""));

            GameObject critText = Instantiate(textPrefab, transform.position, quaternion.identity);
            critText.transform.localScale *= 1.75f;
            TextMeshPro text1 = critText.transform.GetChild(0).GetComponent<TextMeshPro>();
            text1.SetText("CRIT!");
            text1.fontStyle = FontStyles.Bold | FontStyles.Italic;
            text1.color = Color.red;

            GameObject damageText = Instantiate(textPrefab, textPosition, quaternion.identity);
            damageText.transform.localScale *= 1.25f;
            TextMeshPro text2 = damageText.transform.GetChild(0).GetComponent<TextMeshPro>();
            text2.SetText($"-{damage}");
            text2.fontStyle = FontStyles.Bold | FontStyles.Italic;
            text2.color = Color.red;
        }
        else
        {
            Debug.Log($"{damage} damage done: {baseDamage} from base, {level} from levels" + (damagePowerUp > 0 ? $", boosted by x{damageMuliplier}" : ""));
            GameObject damageText = Instantiate(textPrefab, textPosition, quaternion.identity);
            TextMeshPro text = damageText.transform.GetChild(0).GetComponent<TextMeshPro>();
            text.SetText($"-{damage}");
            text.color = Color.yellow;
        }
        return damage;
    }

    void RevolverShootingBehaviour(float upgradedFireRate)
    {
        if (timeUntilReloaded <= 0)
        {
            sa.PlayShootingAnimation("shoot 0_2");
            SoundManager.Instance.PlaySound("Shotgun");

            GameObject center = Instantiate(rocketPrefab, transform.position, transform.rotation);
            Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, -12f);
            GameObject left = Instantiate(sniperShotPrefab, transform.position, leftRotation);
            Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, 12f);
            GameObject right = Instantiate(sniperShotPrefab, transform.position, rightRotation);

            if (attackSizeLength > 0)
            {
                center.transform.localScale *= attackSizeMultiplier;
                left.transform.localScale *= attackSizeMultiplier;
                right.transform.localScale *= attackSizeMultiplier;
            }

            float secondsPerShot = 1 / (upgradedFireRate * 4);
            timeUntilReloaded += secondsPerShot;
            currentAmmo++;
        }
    }

    public void IncreaseAmmo(int ammo)
    {
        currentAmmo += ammo;

        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"+{ammo} Ammo");
    }

    public void AttackSizeTimer(float time = 7)
    {
        attackSizeLength = time;

        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"{attackSizeMultiplier}x Damage for {time}s");
    }

    public void DamageUp(float time = 7f)
    {
        damagePowerUp = time;

        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"{damageMuliplier}x Damage for {time}s");
    }

    public void UnlimitedAmmo(float time = 5f)
    {
        unlimitedAmmoTimer = time;

        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"Unlimited Ammo for {time}s");
    }

    public void DoubleCritRate(float time = 7f)
    {
        doubleCritTimer = time;

        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"2x Crit Rate for {time}s");
    }

    public void SpawnFireRateText(float time)
    {
        GameObject pickupText = Instantiate(textPrefab, transform.position, quaternion.identity);
        TextMeshPro text = pickupText.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.SetText($"Increased Fire Rate for {time}s");
    }
}

// Old Auto Aim Code
// void AutoAimandShoot()
// {
//     FindNearestEnemy();

//     if (targetEnemy != null)
//     {
//         // Calculate direction in 2D
//         Vector2 direction = (targetEnemy.position - transform.position).normalized;
//         // Calulate angle and rotate only around Z axis
//         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
//         transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

//         //Shoot if reloaded
//         switch (shooting)
//         {
//             case ShootingBehavours.Basic:
//                 BasicShootingBehaviour();
//                 break;
//             case ShootingBehavours.Spread:
//                 SpreadShootingBehaviour();
//                 break;
//             case ShootingBehavours.Rocket:
//                 RocketShootingBehaviour();
//                 break;
//             case ShootingBehavours.AR:
//                 ARShootingBehaviour();
//                 break;
//             case ShootingBehavours.Sniper:
//                 SniperShootingBehaviour();
//                 break;
//         }
//     }
// }

// void FindNearestEnemy()
// {
//     GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
//     float closestDistance = detectionRange;
//     targetEnemy = null;

//     foreach (GameObject enemy in enemies)
//     {
//         float distance = Vector3.Distance(transform.position, enemy.transform.position);
//         if (distance < closestDistance)
//         {
//             closestDistance = distance;
//             targetEnemy = enemy.transform;
//         }
//     }
// }