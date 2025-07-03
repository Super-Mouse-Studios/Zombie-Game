using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthdrop : MonoBehaviour
{

    public GameObject[] healthDrops;
    [Range(0f, 1f)]
    public float healthDropChance = 0.2f;

    public void Die()
    {
        if (healthDrops != null && healthDrops.Length > 0 && Random.value < healthDropChance)
        {
            int index = Random.Range(0, healthDrops.Length);
            Instantiate(healthDrops[index], transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

