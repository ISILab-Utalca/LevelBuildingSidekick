using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using Newtonsoft.Json;
using LBS.AI;
using System;

/*
[CreateAssetMenu(fileName = "New LBS Layer Assistant", menuName = "ISILab/LBS Layer Assistant")]
[System.Serializable]
public class LBSLayerAssistant : ScriptableObject
{
    [SerializeField, SerializeReference]
    Generator3D generator;

    [SerializeField, SerializeReference]
    List<LBSAIAgent> aiAgents = new List<LBSAIAgent>();
    
    public Generator3D Generator
    {
        get => generator;
        set => generator = value;
    }

    public int AgentsCount => aiAgents.Count;

    public bool AddAgent(LBSAIAgent aiAgent)
    {
        if (aiAgents.Contains(aiAgent))
        {
            return false;
        }
        aiAgents.Add(aiAgent);
        return true;
    }

    public bool RemoveAgent(LBSAIAgent aiAgent)
    {
        var b = aiAgents.Remove(aiAgent);
        return b;
    }

    public LBSAIAgent RemoveAgentAt(int index)
    {
        var aiAgent = aiAgents[index];
        aiAgents.RemoveAt(index);
        return aiAgent;
    }

    public LBSAIAgent GetAgent(int index)
    {
        return aiAgents[index];
    }

    public T GetAgent<T>(string ID = "") where T : LBSAIAgent
    {
        var t = typeof(T);
        foreach (var aiAgent in aiAgents)
        {
            if (aiAgent is T || Utility.Reflection.IsSubclassOfRawGeneric(t, aiAgent.GetType()))
            {
                if (ID.Equals("") || aiAgent.ID.Equals(ID))
                {
                    return aiAgent as T;
                }
            }
        }
        return null;
    }

    public object GetAgent(Type type, string ID = "")
    {
        foreach (var aiAgent in aiAgents)
        {
            var x = aiAgent.GetType();
            var xx = aiAgent.GetType().BaseType;

            if (aiAgent.GetType().Equals(type) || Utility.Reflection.IsSubclassOfRawGeneric(type, aiAgent.GetType()))
            {
                if (ID.Equals("") || aiAgent.ID.Equals(ID))
                {
                    return aiAgent;
                }
            }
        }
        return null;

    }

    public List<T> GetAgents<T>(string ID = "") where T : LBSAIAgent
    {
        List<T> agents = new List<T>();
        foreach (var aiAgent in aiAgents)
        {
            if (ID.Equals("") || aiAgent.ID.Equals(ID))
                agents.Add(aiAgent as T);
        }
        return agents;
    }
}
*/
