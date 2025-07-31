using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; } // Makes this class a singleton

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve; // Curve for experience needed to level up; Change in inspector

    int currentLevel, totalExperience;
    int previousLevelExperience, nextLevelsExperience;
    int maxLevel = 99; // Make sure this lines up with experience curve from inspector

    [Header("UI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;

    // [Header("Scripts")]
    private Player_Movement playerHP;
    // [SerializeField] Image experienceBar; // Bar to show the progress of experience

    // Flag to check if the player leveled up this round
    private bool leveledUpThisRound = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy this object if an instance already exists.
        }

        UpdateLevel();
    }

    // Adds EXP gained 
    public void AddExperience(int experience)
    {
        totalExperience += experience;
        Debug.Log($"Experience gained: {experience}, Total Experience: {totalExperience}");

        CheckForLevelUp();
        UpdateInterface();
    }

    // Checks if the player has enough experience to level up
    private void CheckForLevelUp()
    {
        // Finds script
        if (playerHP == null)
            playerHP = FindObjectOfType<Player_Movement>();

        // Updates level accordingly
        while (totalExperience >= nextLevelsExperience)
        {
            ++currentLevel; // Increase level
            leveledUpThisRound = true; // Mark that a level up happened this round
            Debug.Log($"Current Level: {currentLevel}");

            UpdateLevel();

            // Increases player health
            if (playerHP != null)
                playerHP.LevelUp();
            else
            {
                Debug.Log("Script not found");
            }

            // Add level up SFX or VFX below 
            SoundManager.Instance.PlaySound("LevelUp");
        }
    }

    // Returns if a level up occurred this round
    public bool DidLevelOccurThisRound()
    {
        return leveledUpThisRound;
    }

    // Resets the flag for the next round
    public void ResetLevelUpFlag()
    {
        leveledUpThisRound = false;
    }

    // Updates the level based on the total experience curve
    private void UpdateLevel()
    {
        // Type casts and evaluates the curve to get EXP needed
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);

        // Prevents crashing when exceeding max level
        if (currentLevel >= maxLevel)
        {
            nextLevelsExperience = int.MaxValue; // Disables further leveling
        }
        else
        {
            nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        }

        // Update the level text in the UI
        UpdateInterface();
    }

    // UI update method
    private void UpdateInterface()
    {
        int start = totalExperience - previousLevelExperience; // Amount of EXP currently held
        int end = nextLevelsExperience - previousLevelExperience; // Amount of EXP needed for next level

        levelText.text = $"Level: {currentLevel}"; // Update level text

        if (nextLevelsExperience != int.MaxValue)
            experienceText.text = $"Experience: {start}/{end}"; // Update experience text
        else
        {
            experienceText.text = "Max EXP Reached";
        }

        // experienceBar.fillAmount = (float)start / end; // Update experience bar fill amount (if using a bar)
    }

    // For testing purposes; delete later
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddExperience(10);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            AddExperience(50000);
        }
    }

    public int GetCurrentLevel() { return currentLevel; }
    public void ResetLevels()
    {
        currentLevel = 0;
        totalExperience = 0;
        previousLevelExperience = 0;
        nextLevelsExperience = (int)experienceCurve.Evaluate(1); // initialize next level EXP threshold

        UpdateInterface();  // refresh UI display
    }

    public void HideText()
    {
        if (levelText != null)
            levelText.gameObject.SetActive(false);

        if (experienceText != null)
            experienceText.gameObject.SetActive(false);
    }

    public void ShowText()
    {
        if (levelText != null)
            levelText.gameObject.SetActive(true);

        if (experienceText != null)
            experienceText.gameObject.SetActive(true);
    }
}
