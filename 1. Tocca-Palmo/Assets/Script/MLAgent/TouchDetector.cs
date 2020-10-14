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
        foreach(ContactPoint contact in collision.contacts)
        {
            if (string.Compare(contact.otherCollider.name, target.name) == 0)
            {
                havetouch = true;
            }
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
