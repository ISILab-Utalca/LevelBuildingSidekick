using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Generator;
using Newtonsoft.Json;

public class LBSLayerAssistant : ScriptableObject
{
    [SerializeField, SerializeReference]
    Generator3D generator;



    public Generator3D Generator
    {
        get => generator;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
