using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SSmanager : MonoBehaviour
{
    [SerializeField] Button Startbutton;
    [SerializeField] Button Restartbutton;

    // Start is called before the first frame update
    void Start()
    {
        if (Startbutton != null)
            Startbutton.onClick.AddListener(LoadZombieGameScene);

        if (Restartbutton != null)
            Restartbutton.onClick.AddListener(LoadTitleScreen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadZombieGameScene()
    {
        SceneManager.LoadScene("ZombieGameScene"); 
    }

    public static void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene"); 
    }
    public static void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen"); 
    }

}
