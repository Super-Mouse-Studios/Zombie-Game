using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void RePlay()
    {
        SceneManager.LoadScene("ZombieGameScene");

    }

    public void Exit()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
