using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwareness : MonoBehaviour
{

    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField]
    private float PlayerAwarenesDistance;
    private Transform player;
    // Start is called before the first frame update
    private void Awake()
    {
        player= FindObjectOfType<Player_Movement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 enemyToPlayerVector =player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;

        if(enemyToPlayerVector.magnitude <= PlayerAwarenesDistance )
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer= false;
        }
    }
}
