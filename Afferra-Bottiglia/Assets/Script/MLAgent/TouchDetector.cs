using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchDetector : MonoBehaviour
{
    public GameObject target;

    private Boolean havetouch;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.gameObject==target)
        {
            Debug.Log("Settata");
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
