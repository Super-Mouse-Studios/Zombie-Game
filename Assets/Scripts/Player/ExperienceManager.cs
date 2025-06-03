using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; } // Makes this class a singleton

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve; // Curve for experience needed to level up; Change in inspector

    int currentLevel, totalExperience;
    int previousLevelExperience, nextLevelsExperience;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    // [SerializeField] Image experienceBar; // Bar to show the progress of experience

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
        // Updates level accordingly
        while (totalExperience >= nextLevelsExperience)
        {
            ++currentLevel; // Increase level

            UpdateLevel();

            // Add level up SFX or VFX below 
        }
    }

    // Updates the level based on the total experience curve
    private void UpdateLevel()
    {
        // Type casts and evaluates the curve to get EXP needed
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);

        // Update the level text in the UI
        UpdateInterface();
    }

    // UI update method
    private void UpdateInterface()
    {
        int start = totalExperience - previousLevelExperience; // Amount of EXP currently held
        int end = nextLevelsExperience - previousLevelExperience; // Amount of EXP needed for next level

        levelText.text = $"Level: {currentLevel}"; // Update level text
        experienceText.text = $"Experience: {start}/{end}"; // Update experience text

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
            AddExperience(500);
        }
    }
}
