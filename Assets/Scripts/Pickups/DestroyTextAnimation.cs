using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTextAnimation : MonoBehaviour
{
    void Start()
    {
        // GameObject parent = gameObject.transform.parent.gameObject;
        Destroy(gameObject, .5f);
    }
}
