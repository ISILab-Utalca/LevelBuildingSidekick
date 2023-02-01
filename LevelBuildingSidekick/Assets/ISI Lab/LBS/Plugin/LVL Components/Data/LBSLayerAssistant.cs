using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using Newtonsoft.Json;
using LBS.AI;

public class LBSLayerAssistant : ScriptableObject
{
    [SerializeField, SerializeReference]
    Generator3D generator;

    [SerializeField, SerializeReference]
    List<LBSAIAgent> aiAgents;

    public Generator3D Generator
    {
        get => generator;
    }


}
