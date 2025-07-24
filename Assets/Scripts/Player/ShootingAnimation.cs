using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAnimation : MonoBehaviour
{
    public Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
    }

    public void PlayAnimation(string name) { an.Play(name); }

    private IEnumerator PlayShootAnimtion(string name)
    {
        float shootAnimationLength;
        string loopAnimation;

        switch (name)
        {
            case "shot ": // pistol shooting animation
                loopAnimation = "flicker";
                shootAnimationLength = .5f;
                break;
            case "Shoot": // Sniper shot
                loopAnimation = "Flicker";
                shootAnimationLength = 1.3f;
                break;
            case "shoot 0_3": // shotgun
                loopAnimation = "flicker 0_3";
                shootAnimationLength = 1f;
                break;
            case "shoot 0_4": // AR
                loopAnimation = "flicker 0_4";
                shootAnimationLength = .4f;
                break;
            case "shoot 0_5": // Rocket
                loopAnimation = "flicker 0_5";
                shootAnimationLength = 1.2f;
                break;
            case "shoot 0_2": // Revolver
                loopAnimation = "flicker 0_2";
                shootAnimationLength = .93f;
                break;
            default:
                Debug.LogWarning($"{name} is invalid");
                yield break;
        }

        PlayAnimation(loopAnimation);
        yield return new WaitForSeconds(.01f);
        if (name != "Shoot")
            PlayAnimation(name);
        else
            an.Play(name, 0, .17f);

        yield return new WaitForSeconds(shootAnimationLength);

        PlayAnimation(loopAnimation); // returns back to non-shooting animation
    }

    public void PlayShootingAnimation(string name)
    {
        StopAllCoroutines();
        StartCoroutine(PlayShootAnimtion(name));
    }
}
