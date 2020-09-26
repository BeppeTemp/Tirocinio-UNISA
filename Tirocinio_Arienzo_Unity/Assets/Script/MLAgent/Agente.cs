using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Agente : Agent
{
    public GameObject robot_agente;
    public GameObject target;
    public GameObject[] end_effectors;

    private RobotController_1RP rc;
    private TouchDetector td;
    private float[] defaultRotations;
    private int x;

    void Start()
    {
        rc = robot_agente.GetComponent<RobotController_1RP>();
        defaultRotations = new float[32];
    }

    //Quando inizia l'elpisodio fai questo
    public override void OnEpisodeBegin()
    {
        Debug.Log("Ci sono stati : " + x + " tocchi.");
        x = 0;

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

        //Risetto la variabili Booleana
        for (int effectorindex = 0; effectorindex < end_effectors.Length; effectorindex++)
        {
            td = end_effectors[effectorindex].GetComponent<TouchDetector>();
            td.setHaveTouch(false);
        }

        //Sposto il target in una nuova posizione sul tavolo
        target.GetComponent<Transform>().localPosition = new Vector3(11.5f,13.86f,16f);
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
        for (int effectorindex = 0; effectorindex < end_effectors.Length; effectorindex++)
        {
            Vector3 endPosition = end_effectors[effectorindex].transform.position - robot_agente.transform.position;
            sensor.AddObservation(endPosition);
            sensor.AddObservation(targetPosition - endPosition);
        }      
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Movimento giunti
        for (int jointIndex = 0; jointIndex < vectorAction.Length; jointIndex++)
        {
            RotationDirection rotationDirection = ActionIndexToRotationDirection((int)vectorAction[jointIndex]);
            rc.RotateJoint(jointIndex, rotationDirection, false);
        }

        x = 0;
        //Calcolo ricompensa
        float reward = 0;
        for (int effectorindex = 0; effectorindex < 14; effectorindex++)
        {
            td = end_effectors[effectorindex].GetComponent<TouchDetector>();
            if (td.getHaveTouch() == true)
            {
                x++;
                reward += 0.0714f;
            }
        }

        //Termina l'episodio se la distanza fra l'end-effector e il cubo è minore di un tot
        if (reward == 1) 
        {
            SetReward(reward);
            EndEpisode();
        }
        SetReward(reward);
    }

    //Covertitore di ActionIndex in RotationDirection
    static public RotationDirection ActionIndexToRotationDirection(int actionIndex)
    {
        return (RotationDirection)(actionIndex - 1);
    }
}

//Comando di avvio: mlagents-learn config/arm_config.yaml --run-id=BraccioML --force
