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

    private RobotController_1RP rc;
    private TouchDetector td;
    private float[] defaultRotations;

    void Start()
    {
        rc = robot_agente.GetComponent<RobotController_1RP>();
        td = cubo_target.GetComponent<TouchDetector>();
        defaultRotations = new float[32];
    }

    //Quando inizia l'elpisodio fai questo
    public override void OnEpisodeBegin()
    {
        //Ristabilisco la posizione del robot
        defaultRotations[0] = 0.0f;
        defaultRotations[1] = 90f;
        defaultRotations[2] = -90f;
        defaultRotations[3] = 0.0f;
        for (int i = 4; i < 28; i++)
        {
            defaultRotations[i] = 0.0f;
        }
        rc.ForceJointsToRotations(defaultRotations);

        //Risetto la variabile Booleana
        td.setHaveTouch(false);

        //Sposto il cubo in una nuova posizione sul tavolo
        cubo_target.GetComponent<Transform>().localPosition = new Vector3(12, Random.Range(14.45f, 20f), Random.Range(12.5f, 18.15f));
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

        /*Posizione corrente dei giunti
        foreach (var joint in rc.joints)
        {
            sensor.AddObservation(joint.robotPart_1.transform.position - robot_agente.transform.position);
            sensor.AddObservation(joint.robotPart_1.transform.forward);
            sensor.AddObservation(joint.robotPart_1.transform.right);
        }*/

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
            rc.RotateJoint(jointIndex, rotationDirection, false);
        }

        //Termina l'episodio se la distanza fra l'end-effector e il cubo è minore di un tot
        if (td.getHaveTouch()==true)
        {
            SetReward(1f);
            EndEpisode();
        }

        /*Calcolo ricompensa in caso di insuccesso
        float distanceToCube = Vector3.Distance(end_effector.transform.position, cubo_target.transform.position);

        float reward = 1 - (distanceToCube / 10);
        SetReward(reward);*/


        /*var jointHeight = 0f; // This is to reward the agent for keeping high up // max is roughly 3.0f
        for (int jointIndex = 0; jointIndex < rc.joints.Length; jointIndex++)
        {
            jointHeight += rc.joints[jointIndex].robotPart_1.transform.position.y - cubo_target.transform.position.y;
        }
        var reward = -distanceToCube + jointHeight / 100f;

        SetReward(reward * 0.1f);*/

    }

    //Covertitore di ActionIndex in RotationDirection
    static public RotationDirection ActionIndexToRotationDirection(int actionIndex)
    {
        return (RotationDirection)(actionIndex - 1);
    }
}

//Comando di avvio: mlagents-learn config/arm_config.yaml --run-id=BraccioML --force
