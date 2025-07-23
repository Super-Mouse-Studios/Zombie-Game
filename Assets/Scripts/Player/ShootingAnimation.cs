using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAnimation : MonoBehaviour
{
    private Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void PlayAnimation(string name) { an.Play(name); }
}
