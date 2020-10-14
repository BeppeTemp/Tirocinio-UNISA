using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using Unity.Mathematics;

public class Agente : Agent
{
    public GameObject robot_agente;
    public GameObject target;
    public GameObject end_effector;

    private RobotController_1RP rc;
    private TouchDetector td;
    private FallDetector fd;

    private float[] defaultRotations;
    private Quaternion targetDefaultRotation;

    void Start()
    {
        rc = robot_agente.GetComponent<RobotController_1RP>();
        td = target.GetComponent<TouchDetector>();
        fd = target.GetComponent<FallDetector>();

        defaultRotations = new float[4];
        targetDefaultRotation = target.transform.rotation;
    }

    //Quando inizia l'elpisodio fai questo
    public override void OnEpisodeBegin()
    {
        //Ristabilisco la posizione del robot
        defaultRotations[0] = 0.0f;
        defaultRotations[1] = 90f;
        defaultRotations[2] = -90f;
        defaultRotations[3] = 0.0f;
        /*for (int i = 4; i < 28; i++)
        {
            defaultRotations[i] = 0.0f;
        }*/
        rc.ForceJointsToRotations(defaultRotations);

        //Resetto
        target.GetComponent<Transform>().localPosition = new Vector3(0,0,2.25f);
        target.transform.rotation = targetDefaultRotation;
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().freezeRotation = true;
        target.GetComponent<Rigidbody>().freezeRotation = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (rc.joints[0].robotPart_1 == null)
        {
            //Robot non presente, non vengono aggiunte osservazioni
            return;
        }

        //Rotazione corrente dei giunti
        float[] rotations = rc.GetCurrentJointRotations();
        foreach (float rotation in rotations)
        {
            //Normalizza la rotazione in un valore compresto tra -1 e 1
            float normalizedRotation = (rotation / 360.0f) % 1f;
            sensor.AddObservation(normalizedRotation);
        }

        //Posizione corrente dei giunti
        foreach (var joint in rc.joints)
        {
            sensor.AddObservation(joint.robotPart_1.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_1.transform.forward);
            sensor.AddObservation(joint.robotPart_1.transform.right);
        }

        //Posizione del robot-relativamente al target
        Vector3 targetPosition = target.transform.position - robot_agente.transform.position;
        sensor.AddObservation(targetPosition);

        //Posizione degli end-effector relativamente al cubo
         Vector3 endPosition = end_effector.transform.position - robot_agente.transform.position;
         sensor.AddObservation(endPosition);
         sensor.AddObservation(targetPosition - endPosition);   
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Movimento giunti
        for (int jointIndex = 0; jointIndex < vectorAction.Length; jointIndex++)
        {
            RotationDirection rotationDirection = ActionIndexToRotationDirection((int)vectorAction[jointIndex]);
            rc.RotateJoint(jointIndex, rotationDirection, false);
        }

        //Calcolo ricompensa
        if (td.getHaveTouch() == true)
        {
            Debug.Log("Ha toccato");
            SetReward(1);
            td.setHaveTouch(false);
            EndEpisode();
        }

        if(fd.getIsFallen() == true)
        {
            Debug.LogWarning("Caduto");
            fd.setIsFallen(false);
            EndEpisode();
        }
    }

    //Covertitore di ActionIndex in RotationDirection
    static public RotationDirection ActionIndexToRotationDirection(int actionIndex)
    {
        return (RotationDirection)(actionIndex - 1);
    }
}

//Comando di avvio: mlagents-learn config/arm_config.yaml --run-id=BraccioML --force
