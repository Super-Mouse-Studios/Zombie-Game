using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    Animator an;
    SpriteRenderer sr;
    Rigidbody2D rb;
    [Header("Death Animations")]
    [SerializeField] GameObject basic;
    [SerializeField] GameObject tank;
    [SerializeField] GameObject explosive;
    [SerializeField] GameObject baby;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();

        an.Play("run cycle ");
    }

    // Update is called once per frame
    void Update()
    {
        // Flip sprite based on movement direction
        if (rb.velocity.x > 0.1f)
        {
            sr.flipX = true;
        }
        else if (rb.velocity.x < -0.1f)
        {
            sr.flipX = false;
        }
    }

    public void DeathAnimation()
    {
        Debug.Log("Hello");
        string tag = gameObject.tag;
        GameObject toSpawn = null;

        switch (tag)
        {
            case "BasicEnemy":
                toSpawn = basic;
                break;
            case "ExplosiveEnemy":
                toSpawn = explosive;
                break;
            case "TankEnemy":
                toSpawn = tank;
                break;
            case "BabyEnemy":
                toSpawn = baby;
                break;
            default:
                Debug.LogWarning($"{tag} is invalid");
                break;
        }

        if (toSpawn != null)
        {
            GameObject deathAnimation = Instantiate(toSpawn);
            deathAnimation.transform.localScale = transform.localScale;
            Destroy(deathAnimation, 1f);
        }
    }
}