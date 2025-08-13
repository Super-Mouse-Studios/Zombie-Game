using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SSmanager : MonoBehaviour
{
    [SerializeField] public Button Startbutton;
    [SerializeField] public Button Restartbutton;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button EndButton;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
       // if (Startbutton != null)
       //     Startbutton.onClick.AddListener(LoadZombieGameScene);

       // //if (Restartbutton != null)
       //     Restartbutton.onClick.AddListener(LoadTitleScreen);

        //if (EndButton != null)
          //  EndButton.onClick.AddListener(LoadTitleScreen);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseMenuPanel == null) return;

        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Exit()
    {
        SceneManager.LoadScene("TitleScreen");

        ExperienceManager.Instance.HideText();
        ExperienceManager.Instance.ResetLevels();
    }
    //public void LoadZombieGameScene()
    //{
    //    Debug.Log("Loading Zombie Game Scene");
    //    Time.timeScale = 1f; // Ensure time scale is reset
    //    SceneManager.LoadScene("ZombieGameScene"); 
    //}

    //public static void LoadGameOverScene()
    //{
    //    Debug.Log("Loading Game Over Scene");
    //    Time.timeScale = 1f; // Ensure time scale is reset
    //    SceneManager.LoadScene("GameOverScene"); 
    //}
    //public void LoadTitleScreen()
    //{
    //    Debug.Log("Loading Title Screen");
    //    Time.timeScale = 1f; // Ensure time scale is reset
    //    SceneManager.LoadScene("TitleScreen"); 
    //}

}
