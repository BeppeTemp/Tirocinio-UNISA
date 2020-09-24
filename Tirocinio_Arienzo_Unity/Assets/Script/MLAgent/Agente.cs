using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Agente : Agent
{
    public GameObject robot_agente;
    public GameObject cubo_target;
    public GameObject end_effector;

    private RobotController_1RP rc_1;
    private RobotController_4RP rc_4;
    private RobotController_6RP rc_6;

    void Start()
    {
        rc_1 = robot_agente.GetComponent<RobotController_1RP>();
        rc_4 = robot_agente.GetComponent<RobotController_4RP>();
        rc_6 = robot_agente.GetComponent<RobotController_6RP>();
    }

    //Quando inizia l'elpisodio fai questo
    public override void OnEpisodeBegin()
    {
        //Ristabilisco la posizione del robot
        float[] dr1 = {0.0f, 0.0f, 0.0f, 0.0f};
        rc_1.ForceJointsToRotations(dr1);
        float[] dr4 = {0.0f};
        rc_4.ForceJointsToRotations(dr4);
        float[] dr6 = {0.0f, 0.0f, 0.0f, 0.0f};
        rc_6.ForceJointsToRotations(dr6);

        //Sposto il cubo in una nuova posizione sul tavolo
        cubo_target.GetComponent<Transform>().localPosition = new Vector3(6, 8, Random.value * 4 - 2);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Rotazione corrente dei giunti
        float[] rotations_1 = rc_1.GetCurrentJointRotations();
        foreach (float rotation in rotations_1)
        {
            //Normalizza la rotazione in un valore compresto tra -1 e 1
            float normalizedRotation = (rotation / 360.0f) % 1f;
            sensor.AddObservation(normalizedRotation);
        }
        float[] rotations_4 = rc_1.GetCurrentJointRotations();
        foreach (float rotation in rotations_4)
        {
            //Normalizza la rotazione in un valore compresto tra -1 e 1
            float normalizedRotation = (rotation / 360.0f) % 1f;
            sensor.AddObservation(normalizedRotation);
        }
        float[] rotations_6 = rc_1.GetCurrentJointRotations();
        foreach (float rotation in rotations_6)
        {
            //Normalizza la rotazione in un valore compresto tra -1 e 1
            float normalizedRotation = (rotation / 360.0f) % 1f;
            sensor.AddObservation(normalizedRotation);
        }

        //Posizione corrente dei giunti
        foreach (var joint in rc_1.joints)
        {
            sensor.AddObservation(joint.robotPart_1.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_1.transform.forward);
            sensor.AddObservation(joint.robotPart_1.transform.right);
        }
        foreach (var joint in rc_4.joints)
        {
            sensor.AddObservation(joint.robotPart_1.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_1.transform.forward);
            sensor.AddObservation(joint.robotPart_1.transform.right);

            sensor.AddObservation(joint.robotPart_2.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_2.transform.forward);
            sensor.AddObservation(joint.robotPart_2.transform.right);

            sensor.AddObservation(joint.robotPart_3.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_3.transform.forward);
            sensor.AddObservation(joint.robotPart_3.transform.right);

            sensor.AddObservation(joint.robotPart_4.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_4.transform.forward);
            sensor.AddObservation(joint.robotPart_4.transform.right);
        }
        foreach (var joint in rc_6.joints)
        {
            sensor.AddObservation(joint.robotPart_1.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_1.transform.forward);
            sensor.AddObservation(joint.robotPart_1.transform.right);

            sensor.AddObservation(joint.robotPart_2.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_2.transform.forward);
            sensor.AddObservation(joint.robotPart_2.transform.right);

            sensor.AddObservation(joint.robotPart_3.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_3.transform.forward);
            sensor.AddObservation(joint.robotPart_3.transform.right);

            sensor.AddObservation(joint.robotPart_4.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_4.transform.forward);
            sensor.AddObservation(joint.robotPart_4.transform.right);

            sensor.AddObservation(joint.robotPart_5.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_5.transform.forward);
            sensor.AddObservation(joint.robotPart_5.transform.right);

            sensor.AddObservation(joint.robotPart_6.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_6.transform.forward);
            sensor.AddObservation(joint.robotPart_6.transform.right);
        }

        //Posizione del robot-relativamente al cubo
        Vector3 cubePosition = cubo_target.transform.position - robot_agente.transform.position;
        sensor.AddObservation(cubePosition);

        //Posizione del end-effector relativamente al cubo
        Vector3 endPosition = end_effector.transform.position - robot_agente.transform.position;
        sensor.AddObservation(endPosition);
        sensor.AddObservation(cubePosition - endPosition);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Movimento giunti
        for (int jointIndex = 0; jointIndex < vectorAction.Length; jointIndex++)
        {
            RotationDirection rotationDirection = ActionIndexToRotationDirection((int)vectorAction[jointIndex]);
            rc_1.RotateJoint(jointIndex, rotationDirection, false);
            rc_4.RotateJoint(jointIndex, rotationDirection, false);
            rc_6.RotateJoint(jointIndex, rotationDirection, false);
        }

        float distanceToCube = Vector3.Distance(end_effector.transform.position, cubo_target.transform.position);

        //Termina l'episodio se la distanza fra l'end-effector e il cubo è minore di un tot
        if (distanceToCube < 1f)
        {
            SetReward(1f);
            EndEpisode();
        }

        EndEpisode();
    }

    //Covertitore di ActionIndex in RotationDirection
    static public RotationDirection ActionIndexToRotationDirection(int actionIndex)
    {
        return (RotationDirection)(actionIndex - 1);
    }
}
