using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armyguy : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private GameObject ammoPickUpPrefab;
    [SerializeField] private float despawnDelay = 0.5f; // Delay before despawn after dropping pickup
    [SerializeField] private int numberOfAmmoPickups = 2;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    [SerializeField] private int totalWaypoints = 3; // Set this to the number of waypoints you have
    [SerializeField] private float waypointTolerance = 0.1f;
    private HashSet<int> droppedAtWaypoints = new HashSet<int>();

    private bool hasDropped = false;
    private Animator an;

    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
        waypoints.Clear();
        for (int i = 1; i <= totalWaypoints; i++)
        {
            GameObject wp = GameObject.Find("Destination " + i);
            if (wp != null)
                waypoints.Add(wp.transform);
            else
                Debug.LogWarning($"Destination '{i}' not found in scene!");
        }

        an.speed = 3f;
        an.Play("idle 1");
    }

    void Update()
    {
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

       

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= waypointTolerance)
        {
            currentWaypointIndex++;


            // Drop one ammo at the second waypoint (index 1)
            if (currentWaypointIndex == 2 && !droppedAtWaypoints.Contains(2))
            {
                DropAmmo(1);
                droppedAtWaypoints.Add(2);
            }

            // Drop one ammo at the third waypoint (index 2)
            if (currentWaypointIndex == 3 && !droppedAtWaypoints.Contains(3))
            {
                DropAmmo(1);
                droppedAtWaypoints.Add(3);
            }
            // Destroy army guy at the last waypoint
            if (currentWaypointIndex >= waypoints.Count)
            {
                Destroy(gameObject, despawnDelay);
            }
        }
    }

    public void DropAmmo(int amount)
    {
        if (ammoPickUpPrefab != null)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 offset = Random.insideUnitCircle * 0.5f;
                Instantiate(ammoPickUpPrefab, transform.position + offset, Quaternion.identity);
            }
        }
    }

    public System.Action OnArmyGuyKilled;

    public void KillArmyGuy()
    {
        if (OnArmyGuyKilled != null) OnArmyGuyKilled.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only die if hit by a zombie
        if (collision.GetComponent<Zombie_Following>() != null)
        {
            KillArmyGuy();
        }
    }
}