using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Security.Cryptography;
using System.Collections.Concurrent;

public class smartConveyor : Agent
{
    public int unitNum = 0;
    public int unitType = 0;
    public int currentDest;
    int trnMotorDirection;
    int xfrMotorDirection;
    public Vector3 direction;
    public float speed;
    public photoeye inductPE;
    public photoeye exitPE;
    bool inductPEStatus;
    bool exitPEStatus;
    public bool divertSol;
    public float maxSpeed = 10f;
    public List<GameObject> onBelt;
    public float beltRotation;

    public override void OnEpisodeBegin()
    {
        currentDest = 0;
        direction.x = 0f;
        direction.y = 0f;
        direction.z = 0f;
        speed = 0f;
        divertSol = false;
    }

    void FixedUpdate()
    {
        if (unitType == 0)
        {
            inductPEStatus = false;
            exitPEStatus = false;
        }
        else if (unitType == 1)
        {
            inductPEStatus = false;
            exitPEStatus = exitPE.blocked;
        }
        else if (unitType == 2)
        {
            inductPEStatus = inductPE.blocked;
            exitPEStatus = exitPE.blocked;
        }
    }

    void Update()
    {
        for (int i = 0; i <= onBelt.Count - 1; i++)
        {
            onBelt[i].GetComponent<Rigidbody>().velocity = speed * direction;
            currentDest = onBelt[i].GetComponent<cartonDestination>().dest;
        }
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Unit Number
        sensor.AddObservation(unitNum);

        // Unit Type
        sensor.AddObservation(unitType);

        // Current Carton Destination
        sensor.AddObservation(currentDest);

        // Conveyor X Direction
        sensor.AddObservation(trnMotorDirection);

        // Conveyor Z Direction
        sensor.AddObservation(xfrMotorDirection);

        // Conveyor Speed
        sensor.AddObservation(speed);

        // Induct Photoeye Status
        sensor.AddObservation(inductPEStatus);

        // Exit Photoeye Status
        sensor.AddObservation(exitPEStatus);

        // Divert Solenoid Status
        sensor.AddObservation(divertSol);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Transport Motor Direction Action
        int controlSignaltrnMotorDirection = actionBuffers.DiscreteActions[0];

        if (xfrMotorDirection == 0 && !divertSol)
        {

            if (controlSignaltrnMotorDirection == 0)
            {
                trnMotorDirection = 0;
                direction.x = 0;
                direction.y = 0;
                direction.z = 0;
            }
            else if (controlSignaltrnMotorDirection == 1)
            {
                trnMotorDirection = -1;
                if (beltRotation == 0)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 0 && beltRotation < 90)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
                else if (beltRotation == 90)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = -1;
                }
                else if (beltRotation > 90 && beltRotation < 180)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
                else if (beltRotation == 180)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 180 && beltRotation < 270)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
                else if (beltRotation == 270)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = 1;
                }
                else if (beltRotation > 270 && beltRotation < 360)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
            }
            else if (controlSignaltrnMotorDirection == 2)
            {
                trnMotorDirection = 1;
                if (beltRotation == 0)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 0 && beltRotation < 90)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
                else if (beltRotation == 90)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = 1;
                }
                else if (beltRotation > 90 && beltRotation < 180)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
                else if (beltRotation == 180)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 180 && beltRotation < 270)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
                else if (beltRotation == 270)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = -1;
                }
                else if (beltRotation > 270 && beltRotation < 360)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan(beltRotation * Math.PI / 180f));
                }
            }
        }

        // Transfer Motor Direction Action
        int controlSignalxfrDirection = actionBuffers.DiscreteActions[1];

        if (trnMotorDirection == 0 && divertSol)
        {
            if (controlSignalxfrDirection == 0)
            {
                xfrMotorDirection = 0;
                direction.x = 0;
                direction.y = 0;
                direction.z = 0;
            }
            else if (controlSignalxfrDirection == 1)
            {
                xfrMotorDirection = -1;
                if (beltRotation == 0)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = -1;
                }
                else if (beltRotation > 0 && beltRotation < 90)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
                else if (beltRotation == 90)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 90 && beltRotation < 180)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
                else if (beltRotation == 180)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = -1;
                }
                else if (beltRotation > 180 && beltRotation < 270)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
                else if (beltRotation == 270)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 270 && beltRotation < 360)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
            }
            else if (controlSignalxfrDirection == 2)
            {
                xfrMotorDirection = 1;
                if (beltRotation == 0)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = 1;
                }
                else if (beltRotation > 0 && beltRotation < 90)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
                else if (beltRotation == 90)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 90 && beltRotation < 180)
                {
                    direction.x = 1;
                    direction.y = 0;
                    direction.z = (float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
                else if (beltRotation == 180)
                {
                    direction.x = 0;
                    direction.y = 0;
                    direction.z = 1;
                }
                else if (beltRotation > 180 && beltRotation < 270)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
                else if (beltRotation == 270)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = 0;
                }
                else if (beltRotation > 270 && beltRotation < 360)
                {
                    direction.x = -1;
                    direction.y = 0;
                    direction.z = -(float)(Math.Tan((beltRotation + 90) * Math.PI / 180f));
                }
            }
        }

        // Speed Action
        int controlSignalSpeed = actionBuffers.DiscreteActions[2];

        if (controlSignalSpeed == 0)
        {
            speed = speed;
        }

        else if (controlSignalSpeed == 1)
        {
            if (speed > 0f)
            {
                speed -= 1f;
            }
            else
            {
                speed = 0f;
            }
        }
        else if (controlSignalSpeed == 2)
        {
            if (speed < maxSpeed)
            {
                speed += 1f;
            }
            else
            {
                speed = maxSpeed;
            }
        }

        // Divert Solenoid Action
        int controlSignalDivert = actionBuffers.DiscreteActions[3];

        if (controlSignalDivert == 0)
        {
            divertSol = false;
        }
        else if (controlSignalDivert == 1 && direction.x == 0f && direction.z == 0f)
        {
            divertSol = true;
        }

        // Local Rewards
        if (unitType == 0 || unitType == 1 || unitType == 2)
        {
            if (controlSignalxfrDirection == 1 || controlSignalxfrDirection == 2 || controlSignalDivert == 1)
            {
                AddReward(-1.0f);
            }
        }

    }

    // Detect when an object is on the belt
    private void OnCollisionEnter(Collision collision)
    {
        onBelt.Add(collision.gameObject);
    }

    // Detect when an object leaves the belt
    private void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject);
        currentDest = 0;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        discreteActionsOut[1] = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[1] = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[1] = 2;
        }

        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[2] = 2;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[2] = 1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[3] = 1;
        }
    }
}
