using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
public class DeathMassage : MonoBehaviour
{
    public GameObject Player;

    void Update()
    {
        if (Player == null)
        {
            SceneManager.LoadScene("Endscreen");
        }
    }
}

