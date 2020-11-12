using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TouchDetector : MonoBehaviour
{
    public GameObject target;
    public GameObject end_effector;

    private Boolean havetouch;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.gameObject.tag == end_effector.tag)
        {
            Debug.Log("Toccato");
            havetouch = true;
        }
    }

    public Boolean getHaveTouch()
    {
        return havetouch;
    }

    public void setHaveTouch(Boolean value)
    {
        havetouch = value;
    }
}
