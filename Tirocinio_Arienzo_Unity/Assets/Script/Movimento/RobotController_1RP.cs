using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotController_1RP : MonoBehaviour
{
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart_1;
    }
    public Joint[] joints;


    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart_1 = joints[i].robotPart_1;
            UpdateRotationState(RotationDirection.None, robotPart_1);
        }
    }

    public void RotateJoint(int jointIndex, RotationDirection direction, bool stopPrevious = true)
    {
        if (stopPrevious)
        {
            StopAllJointRotations();
        }
        Joint joint = joints[jointIndex];
        UpdateRotationState(direction, joint.robotPart_1);
    }

    static void UpdateRotationState(RotationDirection direction, GameObject robotPart)
    {
        ArticulationJointController jointController = robotPart.GetComponent<ArticulationJointController>();
        jointController.rotationState = direction;
    }

    public void ForceJointsToRotations(float[] rotations)
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            Joint joint = joints[i];
            ArticulationJointController jointController = joint.robotPart_1.GetComponent<ArticulationJointController>();
            jointController.ForceToRotation(0.0f);
        }
    }

    public float[] GetCurrentJointRotations()
    {


        float[] list = new float[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            Joint joint = joints[i];
            ArticulationJointController jointController = joint.robotPart_1.GetComponent<ArticulationJointController>();
            float currentRotation = jointController.CurrentPrimaryAxisRotation();
            list[i] = currentRotation;
        }
        return list;
    }
}
