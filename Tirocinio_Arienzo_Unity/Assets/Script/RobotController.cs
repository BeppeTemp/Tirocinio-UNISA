using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotController : MonoBehaviour
{
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart_1;
        public GameObject robotPart_2;
        public GameObject robotPart_3;
        public GameObject robotPart_4;
        public GameObject robotPart_5;
        public GameObject robotPart_6;
    }
    public Joint[] joints;


    // CONTROL

    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart_1 = joints[i].robotPart_1;
            UpdateRotationState(RotationDirection.None, robotPart_1);
            GameObject robotPart_2 = joints[i].robotPart_2;
            UpdateRotationState(RotationDirection.None, robotPart_2);
            GameObject robotPart_3 = joints[i].robotPart_3;
            UpdateRotationState(RotationDirection.None, robotPart_3);
            GameObject robotPart_4 = joints[i].robotPart_4;
            UpdateRotationState(RotationDirection.None, robotPart_4);
            GameObject robotPart_5 = joints[i].robotPart_5;
            UpdateRotationState(RotationDirection.None, robotPart_5);
            GameObject robotPart_6 = joints[i].robotPart_6;
            UpdateRotationState(RotationDirection.None, robotPart_6);
        }
    }

    public void RotateJoint(int jointIndex, RotationDirection direction)
    {
        StopAllJointRotations();
        Joint joint = joints[jointIndex];
        UpdateRotationState(direction, joint.robotPart_1);
        UpdateRotationState(direction, joint.robotPart_2);
        UpdateRotationState(direction, joint.robotPart_3);
        UpdateRotationState(direction, joint.robotPart_4);
        UpdateRotationState(direction, joint.robotPart_5);
        UpdateRotationState(direction, joint.robotPart_6);
    }

    // HELPERS

    static void UpdateRotationState(RotationDirection direction, GameObject robotPart)
    {
        ArticulationJointController jointController = robotPart.GetComponent<ArticulationJointController>();
        jointController.rotationState = direction;
    }



}
