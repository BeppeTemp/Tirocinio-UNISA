using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotController_4RP : MonoBehaviour
{
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart_1;
        public GameObject robotPart_2;
        public GameObject robotPart_3;
        public GameObject robotPart_4;

    }
    public Joint[] joints;


    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart_1 = joints[i].robotPart_1;
            UpdateRotationState(RotationDirection.None, robotPart_1);
            GameObject robotPart_2 = joints[i].robotPart_2;
            UpdateRotationState(RotationDirection.None, robotPart_2);
            GameObject robotPart_3 = joints[i].robotPart_2;
            UpdateRotationState(RotationDirection.None, robotPart_3);
            GameObject robotPart_4 = joints[i].robotPart_2;
            UpdateRotationState(RotationDirection.None, robotPart_4);
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
        UpdateRotationState(direction, joint.robotPart_2);
        UpdateRotationState(direction, joint.robotPart_3);
        UpdateRotationState(direction, joint.robotPart_4);
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
            ArticulationJointController jc1 = joint.robotPart_1.GetComponent<ArticulationJointController>();
            jc1.ForceToRotation(0.0f);
            ArticulationJointController jc2 = joint.robotPart_2.GetComponent<ArticulationJointController>();
            jc2.ForceToRotation(0.0f);
            ArticulationJointController jc3 = joint.robotPart_3.GetComponent<ArticulationJointController>();
            jc3.ForceToRotation(0.0f);
            ArticulationJointController jc4 = joint.robotPart_4.GetComponent<ArticulationJointController>();
            jc4.ForceToRotation(0.0f);
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
