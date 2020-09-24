using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManualInput_6RP : MonoBehaviour
{
    public GameObject robot;

    void Update()
    {
        RobotController_6RP robotController = robot.GetComponent<RobotController_6RP>();
        for (int i = 0; i < robotController.joints.Length; i++)
        {
            float inputVal = Input.GetAxis(robotController.joints[i].inputAxis);
            if (Mathf.Abs(inputVal) > 0)
            {
                RotationDirection direction = GetRotationDirection(inputVal);
                robotController.RotateJoint(i, direction);
                return;
            }
        }
        robotController.StopAllJointRotations();
    }

    // HELPERS
    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
        {
            return RotationDirection.Positive;
        }
        else if (inputVal < 0)
        {
            return RotationDirection.Negative;
        }
        else
        {
            return RotationDirection.None;
        }
    }
}