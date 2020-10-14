using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public GameObject target_1;
    public GameObject target_2;

    private Boolean isFallen;

    private void OnCollisionEnter(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            if (string.Compare(contact.thisCollider.name, "Corpo") == 0)
            {
                if ((string.Compare(contact.otherCollider.name, target_1.name) == 0) || (string.Compare(contact.otherCollider.name, target_2.name) == 0))
                {
                    isFallen = true;
                }
            }
            if (string.Compare(contact.thisCollider.name, "Collo") == 0)
            {
                if ((string.Compare(contact.otherCollider.name, target_1.name) == 0) || (string.Compare(contact.otherCollider.name, target_2.name) == 0))
                {
                    isFallen = true;
                }
            }
            if (string.Compare(contact.thisCollider.name, "ColliderBase") == 0)
            {
                if (string.Compare(contact.otherCollider.name, target_2.name) == 0)
                {
                    isFallen = true;
                }
            }
        }
    }

    public void setIsFallen(Boolean value)
    {
        isFallen = value;
    }

    public Boolean getIsFallen()
    {
        return isFallen;
    }
}
