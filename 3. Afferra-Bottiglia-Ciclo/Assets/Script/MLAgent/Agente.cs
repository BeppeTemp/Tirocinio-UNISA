using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class Agente : Agent
{
    public GameObject robot_agente;
    public GameObject target;
    public GameObject end_effector;
    public GameObject[] fingers;

    private RobotController_1RP rc;
    private TouchDetector td;
    private FallDetector fd;

    private float[] defaultRotations;
    private Quaternion targetDefaultRotation;
    private float[] currentRotations;
    private Boolean isinposition;
    private int handiteraction;
    private int armiupteraction;
    private int armrotationiteraction;

    void Start()
    {
        //Inizializzazione controllori
        rc = robot_agente.GetComponent<RobotController_1RP>();
        td = target.GetComponent<TouchDetector>();
        fd = target.GetComponent<FallDetector>();

        //Iniziazlizzazioen vettori di rotazione
        defaultRotations = new float[32];
        targetDefaultRotation = target.transform.rotation;
        currentRotations = new float[4];

        //Inizializzazione del numero di iterazioni dopo il tocco
        handiteraction = 0;
        armiupteraction = 0;
        armrotationiteraction = 0;

        //Inizializzazione della variabile buleana relativa alla posizione di tocco
        isinposition = false;

        //Inizializzazione vettore della rotazione iniziale braccio
        defaultRotations[0] = -30f;
        defaultRotations[1] = 90f;
        defaultRotations[2] = -90f;
        defaultRotations[3] = 0.0f;
        for (int i = 4; i < 28; i++)
        {
            defaultRotations[i] = 0.0f;
        }
    }

    //Quando inizia l'elpisodio fai questo
    public override void OnEpisodeBegin()
    {
        //Ristabilisco la posizione del robot
        rc.ForceJointsToRotations(defaultRotations);

        //Resetto il vettore delle posizioni correnti
        currentRotations[0] = defaultRotations[0];
        currentRotations[1] = defaultRotations[1];
        currentRotations[2] = defaultRotations[2];
        currentRotations[3] = defaultRotations[3];

        //Resetto la posizione della bottiglia
        target.GetComponent<Transform>().localPosition = new Vector3(-0.150f, 0, 2.25f);
        target.GetComponent<Transform>().localPosition = new Vector3(0,0, 2.25f);
        target.transform.rotation = targetDefaultRotation;
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().freezeRotation = true;
        target.GetComponent<Rigidbody>().freezeRotation = false;

        //Reset del numero di iterazioni dopo il tocco
        handiteraction = 0;
        armiupteraction = 0;
        armrotationiteraction = 0;

        //Reset della variabile booleana relativa alla posizione di tocco
        isinposition = false;

        //Reset della variabile booleana realtiva al tocco
        td.setHaveTouch(false);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Se il robot non è presente
        if (rc.joints[0].robotPart_1 == null)
        {
            //Robot non presente, non vengono aggiunte osservazioni
            return;
        }

        //Rotazione corrente dei giunti
        float[] rotations = rc.GetCurrentJointRotations();
        for (int i = 4; i < 4; i++)
        {
            //Normalizza la rotazione in un valore compresto tra -1 e 1
            float normalizedRotation = (rotations[i] / 360.0f) % 1f;
            sensor.AddObservation(normalizedRotation);
        }

        //Posizione del end-effector e target
        sensor.AddObservation(target.transform.position);
        sensor.AddObservation(end_effector.transform.position);

        //Posizione del end-effector relativamente al target
        Vector3 distance = end_effector.transform.position - target.transform.position;
        sensor.AddObservation(distance);

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Calcolo ricompensa
        if (td.getHaveTouch() == true)
        {
            if (isinposition == false)
            {
                rc.RotateJointToRotation(0, currentRotations[0]);
                rc.RotateJointToRotation(1, currentRotations[1]);
                rc.RotateJointToRotation(2, currentRotations[2]);
                rc.RotateJointToRotation(3, currentRotations[3]);
                isinposition = true;
                Debug.Log("Ha toccato");
            }

            if (handiteraction < 25)
            {
                handClose();
                handiteraction++;
            }
            if(handiteraction >= 25)
            {
                if (armiupteraction < 100)
                {
                    armUP();
                    armiupteraction++;
                }
                if (armiupteraction >= 100)
                {
                    if (armrotationiteraction < 200)
                    {
                        armRotation();
                        armrotationiteraction++;
                    }
                    if (armrotationiteraction >= 200)
                    {
                        if (handiteraction > 0)
                        {
                            handOpen();
                            handiteraction--;
                        }
                    }     
                }
            }

            /*Sezione di addestramento
            Debug.Log("Ha toccato");
            SetReward(1);
            td.setHaveTouch(false);
            EndEpisode();*/
        }
        else
        {
            //Movimento giunti
            for (int jointIndex = 0; jointIndex < vectorAction.Length; jointIndex++)
            {
                RotationDirection rotationDirection = ActionIndexToRotationDirection((int)vectorAction[jointIndex]);
                rc.RotateJoint(jointIndex, rotationDirection, false);
                currentRotations[jointIndex] = rc.GetCurrentJointRotations(jointIndex);
            }
        }

        //Controllo sullo stato della bottiglia durante l'addestramento e la simulazione
        if (fd.getIsFallen() == true)
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

    //Movimento di chiusura della mano
    private void handClose()
    {
            rc.RotateJoint(4, RotationDirection.Positive, false);
            rc.RotateJoint(5, RotationDirection.Positive, false);
            rc.RotateJoint(6, RotationDirection.Positive, false);
            rc.RotateJoint(7, RotationDirection.Positive, false);
            rc.RotateJoint(8, RotationDirection.Positive, false);
            rc.RotateJoint(9, RotationDirection.Positive, false);

            rc.RotateJoint(10, RotationDirection.Positive, false);
            rc.RotateJoint(11, RotationDirection.Positive, false);
            rc.RotateJoint(12, RotationDirection.Positive, false);
            rc.RotateJoint(13, RotationDirection.Positive, false);
            rc.RotateJoint(14, RotationDirection.Positive, false);
            rc.RotateJoint(15, RotationDirection.Positive, false);

            rc.RotateJoint(16, RotationDirection.Positive, false);
            rc.RotateJoint(17, RotationDirection.Positive, false);
            rc.RotateJoint(18, RotationDirection.Positive, false);
            rc.RotateJoint(19, RotationDirection.Positive, false);
            rc.RotateJoint(20, RotationDirection.Positive, false);
            rc.RotateJoint(21, RotationDirection.Positive, false);

            rc.RotateJoint(22, RotationDirection.Positive, false);
            rc.RotateJoint(23, RotationDirection.Positive, false);
            rc.RotateJoint(24, RotationDirection.Positive, false);
            rc.RotateJoint(25, RotationDirection.Positive, false);
            rc.RotateJoint(26, RotationDirection.Positive, false);
            rc.RotateJoint(27, RotationDirection.Positive, false);
    }

    //Movimento di apertura della mano
    private void handOpen()
    {
            rc.RotateJoint(4, RotationDirection.Negative, false);
            rc.RotateJoint(5, RotationDirection.Negative, false);
            rc.RotateJoint(6, RotationDirection.Negative, false);
            rc.RotateJoint(7, RotationDirection.Negative, false);
            rc.RotateJoint(8, RotationDirection.Negative, false);
            rc.RotateJoint(9, RotationDirection.Negative, false);

            rc.RotateJoint(10, RotationDirection.Negative, false);
            rc.RotateJoint(11, RotationDirection.Negative, false);
            rc.RotateJoint(12, RotationDirection.Negative, false);
            rc.RotateJoint(13, RotationDirection.Negative, false);
            rc.RotateJoint(14, RotationDirection.Negative, false);
            rc.RotateJoint(15, RotationDirection.Negative, false);

            rc.RotateJoint(16, RotationDirection.Negative, false);
            rc.RotateJoint(17, RotationDirection.Negative, false);
            rc.RotateJoint(18, RotationDirection.Negative, false);
            rc.RotateJoint(19, RotationDirection.Negative, false);
            rc.RotateJoint(20, RotationDirection.Negative, false);
            rc.RotateJoint(21, RotationDirection.Negative, false);

            rc.RotateJoint(22, RotationDirection.Negative, false);
            rc.RotateJoint(23, RotationDirection.Negative, false);
            rc.RotateJoint(24, RotationDirection.Negative, false);
            rc.RotateJoint(25, RotationDirection.Negative, false);
            rc.RotateJoint(26, RotationDirection.Negative, false);
            rc.RotateJoint(27, RotationDirection.Negative, false);
    }

    //Movimento di sollevamento del braccio
    private void armUP()
    {
        rc.RotateJoint(2, RotationDirection.Negative, false);
    }

    //Movimento di rotazione della spalla
    private void armRotation()
    {
        rc.RotateJoint(0, RotationDirection.Negative, false);
    }
}

//Comando di avvio: mlagents-learn config/arm_config.yaml --run-id=BraccioML --force
