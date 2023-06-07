using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
[LBSCharacteristic("Directions", "")]
public class LBSDirection : LBSCharacteristic, ICloneable
{
    #region SUB-STRUCTURE
    [System.Serializable]
    public class weightStruct
    {
        [SerializeField]
        public GameObject target;

        [Range(0f, 1f)]
        public float weigth;
    };
    #endregion

    #region FIELDS
    [Tooltip("4-Conected: 0: Foward, 1: Rigth, 2: Bottom, 3: Left")]
    [ScriptableObjectReference(typeof(LBSIdentifier))]
    [SerializeField, JsonRequired]
    private List<string> connections = new List<string>();

    [JsonRequired,SerializeField]
    private int connectionAmount = 4;

    [JsonIgnore, SerializeField]
    private List<weightStruct> weights = new List<weightStruct>();
    #endregion

    #region PROPERTIES
    public List<string> Connections => new List<string>(connections);

    [SerializeField]
    public List<weightStruct> Weights => new List<weightStruct>(weights); 

    public float TotalWeight => weights.Sum( w => w.weigth);
    #endregion

    #region CONSTRUCTORS
    public LBSDirection() : base() { }

    public LBSDirection(string label,List<string> tags) : base(label) 
    {
        this.connections = tags;
    }
    #endregion

    #region METHODS
    public string[] GetConnection(int rotation = 0)
    {
        var conections = connections;

        for (int i = 0; i < rotation; i++)
        {
            conections = conections.Rotate();
        }

        return conections.ToArray();
    }

    public override object Clone()
    {
        return new LBSDirection(this.label, new List<string>(this.connections));
    }

    public override void OnEnable()
    {
        foreach (var asset in Owner.Assets)
        {
            weights.Add(new weightStruct() 
            {
                target = asset,
                weigth = 1f 
            });
        }

        Owner.OnAddAsset += (asset) =>
            {
                weights.Add(new weightStruct()
                {
                    target = asset,
                    weigth = 1f
                });
            };
    }
    #endregion

}

