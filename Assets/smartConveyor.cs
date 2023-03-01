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
    public Vector3 direction;
    public float speed;
    public photoeye inductPE;
    bool inductPEStatus;
    bool exitPEStatus;
    public photoeye exitPE;
    public bool divertSol;
    public float maxSpeed = 10f;
    public List<GameObject> onBelt;

    public override void OnEpisodeBegin()
    {
        currentDest = 0;
        direction.x = 0f;
        direction.z = 0f;
        speed = 0f;
        divertSol = false;
    }

    void Update()
    {
        for (int i = 0; i <= onBelt.Count - 1; i++)
        {
            if (unitType == 0)
            {
                inductPEStatus = false;
                exitPEStatus = false;
            }
            else if (unitType == 1)
            {
                inductPEStatus = inductPE.blocked;
                exitPEStatus = exitPE.blocked;
            }

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
        sensor.AddObservation(direction.x);

        // Conveyor Z Direction
        sensor.AddObservation(direction.z);

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
        // X Direction Action
        int controlSignalXDirection = actionBuffers.DiscreteActions[0];

        if (direction.z == 0f && !divertSol)
        {
            if (controlSignalXDirection == 0)
            {
                direction.x = 0f;
            }
            else if (controlSignalXDirection == 1)
            {
                direction.x = -1f;
            }
            else if (controlSignalXDirection == 2)
            {
                direction.x = 1f;
            }
        }

        // Y Direction Action
        int controlSignalYDirection = actionBuffers.DiscreteActions[1];

        if (direction.x == 0f && divertSol)
        {
            if (controlSignalYDirection == 0)
            {
                direction.z = 0f;
            }
            else if (controlSignalYDirection == 1)
            {
                direction.z = -1f;
            }
            else if (controlSignalYDirection == 2)
            {
                direction.z = 1f;
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
