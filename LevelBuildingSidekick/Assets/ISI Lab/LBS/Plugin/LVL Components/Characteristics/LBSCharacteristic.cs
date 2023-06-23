using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

/// <summary>
/// Requiere que las cosas que hereden de el tengan un contructor por defecto sin parametros
/// </summary>
[System.Serializable]
public abstract class LBSCharacteristic : ICloneable
{
    [HideInInspector, JsonIgnore]
    private Bundle owner;
    [HideInInspector, JsonIgnore]
    public Bundle Owner => owner;

    [JsonRequired, SerializeField]
    protected string label = "";

    [JsonIgnore]
    public string Label
    {
        get => label;
        set => label = value;
    }

    public LBSCharacteristic() { }

    public LBSCharacteristic(string label)
    {
        this.label = label;
    }

    public abstract object Clone();

    public override bool Equals(object obj)
    {
        return base.Equals(obj); //obj is LBSCharacteristic && (obj as LBSCharacteristic)?.label == label;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// esta funcion es para que la characteristic tenga axeso a el bundle que lo posee
    /// asi podemos tener acciones o itenracciones dentro bharacteristics
    /// </summary>
    public void Init(Bundle owner)
    {
        this.owner = owner;
        OnEnable();
    }

    public abstract void OnEnable();

}
