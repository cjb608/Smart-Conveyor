using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Security.Cryptography;
using System.ComponentModel;

public class smartConveyorEnvController : MonoBehaviour
{
    [System.Serializable]
    public class AgentInfo
    {
        public smartConveyor Agent;
    }

    [Header("Max Environment Steps")] 
    public int MaxEnvironmentSteps = 500;

    public List<AgentInfo> AgentsList = new List<AgentInfo>();
    public Transform Carton;
    public int numCartonsOnBelt = 0;
    private SimpleMultiAgentGroup m_AgentGroup;
    private int m_ResetTimer;
    public cartonDestination cartonDest;


    void Start()
    {
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            m_AgentGroup.RegisterAgent(item.Agent);
        }

        ResetEnvironment();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        m_AgentGroup.AddGroupReward(-1.0f / MaxEnvironmentSteps);
        if (m_ResetTimer > MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_AgentGroup.GroupEpisodeInterrupted();
            ResetEnvironment();
        }

        numCartonsOnBelt = 0;
        foreach (var item in AgentsList)
        {
            numCartonsOnBelt += item.Agent.onBelt.Count;
        }

        if (numCartonsOnBelt == 0)
        {
            m_AgentGroup.SetGroupReward(-100.0f);
            m_AgentGroup.EndGroupEpisode();
            ResetEnvironment();
        }

        if (cartonDest.dest == 0)
        {
            if (AgentsList[0].Agent.exitPE.blocked)
            {
                m_AgentGroup.SetGroupReward(1.0f);
                m_AgentGroup.EndGroupEpisode();
                ResetEnvironment();
            }
        }
        else if (cartonDest.dest == 1)
        {
            if (AgentsList[1].Agent.exitPE.blocked)
            {
                m_AgentGroup.SetGroupReward(1.0f);
                m_AgentGroup.EndGroupEpisode();
                ResetEnvironment();
            }
        }
        else if (cartonDest.dest == 2)
        {
            if (AgentsList[3].Agent.exitPE.blocked)
            {
                m_AgentGroup.SetGroupReward(1.0f);
                m_AgentGroup.EndGroupEpisode();
                ResetEnvironment();
            }
        }
    }

    public void ResetEnvironment()
    {
        int ranNum = UnityEngine.Random.Range(0, 3);

        // Reset the carton to the start of the belt
        Carton.localPosition = new Vector3(-0.25f, 1.75f, 0f);
        Carton.localRotation = Quaternion.Euler(0f, 0f, 0f);
        cartonDest.dest = ranNum;

        // Reset Timer
        m_ResetTimer = 0;
    }
}
