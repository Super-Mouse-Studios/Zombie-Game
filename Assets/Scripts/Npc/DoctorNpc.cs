using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorNpc : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameObject healthPickUpPrefab;
    [SerializeField] private Vector2 moveDirection = Vector2.right; // Set direction in Inspector or at spawn
    [SerializeField] private float despawnDelay = 0.5f; // Delay before despawn after dropping pickup
    [SerializeField] private int numberOfHealthPickups = 1;
    [SerializeField] private float minX = -10f; // Minimum X position for despawn
    [SerializeField] private float maxX = 10f; // Maximum X position for despawn
    [SerializeField] private float runDuration = 3f; // How long the doctor runs before dropping
    [SerializeField] private float postDropDuration = 2f; // How long doctor keeps moving after dropping
    private float postDropTimer = 0f;



    private float runTimer = 0f;
    private bool hasDropped = false;

    private bool canRespawn = true;
    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)moveDirection.normalized * moveSpeed * Time.deltaTime;
        runTimer += Time.deltaTime;

        // Drop health after runDuration
        if (!hasDropped && runTimer >= runDuration)
        {
            DropHealth();
        }

        // After dropping, start post-drop timer
        if (hasDropped)
        {
            postDropTimer += Time.deltaTime;

            // Despawn after postDropDuration (ignore bounds until timer is up)
            if (postDropTimer >= postDropDuration)
            {
                Destroy(gameObject, despawnDelay);
            }
        }
    }
    public void DropHealth()
    {
        if (hasDropped) return; // Prevent multiple drops
        hasDropped = true;
        postDropTimer = 0f; // Start post-drop timer

        if (healthPickUpPrefab != null)
        {
            for (int i = 0; i < numberOfHealthPickups; i++)
            {
                Vector3 offset = Random.insideUnitCircle * 0.5f;
                Instantiate(healthPickUpPrefab, transform.position + offset, Quaternion.identity);
            }
        }
    }
    public System.Action OnDoctorKilled;

    public void KillDoctor()
    {
        canRespawn = false;
        if (OnDoctorKilled != null) OnDoctorKilled.Invoke();
        Destroy(gameObject);
    }

    public bool CanRespawn()
    {
        return canRespawn;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only die if hit by a zombie
        if (collision.GetComponent<Zombie_Following>() != null)
        {
            KillDoctor();
        }
    }
}

