using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateCollider : MonoBehaviour
{
    public GameObject target;
    void Start()
    {
        target.GetComponent<Collider>().enabled = false;
    }
}
