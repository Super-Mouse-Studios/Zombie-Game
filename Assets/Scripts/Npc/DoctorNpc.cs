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
    //[SerializeField] private float minX = -10f; // Minimum X position for despawn
    //[SerializeField] private float maxX = 10f; // Maximum X position for despawn
    [SerializeField] private float runDuration = 3f; // How long the doctor runs before dropping
    [SerializeField] private float postDropDuration = 2f; // How long doctor keeps moving after dropping
    private float postDropTimer = 0f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    [SerializeField] private int totalWaypoints = 3; // Set this to the number of waypoints you have
    [SerializeField] private float waypointTolerance = 0.1f;

    private float runTimer = 0f;
    private bool hasDropped = false;

    private bool canRespawn = true;
    private Animator an;


    void Start()
    {
        an = GetComponent<Animator>();
        waypoints.Clear();
        for (int i = 1; i <= totalWaypoints; i++)
        {
            GameObject wp = GameObject.Find("Wayfinder " + i);
            if (wp != null)
                waypoints.Add(wp.transform);
            else
                Debug.LogWarning($"Waypoint 'Wayfinder {i}' not found in scene!");
        }

        an.speed = 3f;
        an.Play("Idle 2");
    }
    // Update is called once per frame
    void Update()
    {
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= waypointTolerance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                DropHealth();
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

