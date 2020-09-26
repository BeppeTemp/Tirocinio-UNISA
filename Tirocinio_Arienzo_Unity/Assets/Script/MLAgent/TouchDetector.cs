using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TouchDetector : MonoBehaviour
{
    public GameObject target;

    private Boolean havetouch = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.gameObject==target)
        {
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
