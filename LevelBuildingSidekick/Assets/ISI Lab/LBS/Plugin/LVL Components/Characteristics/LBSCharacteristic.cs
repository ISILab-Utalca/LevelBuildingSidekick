using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using LBS.Bundles;

/// <summary>
/// Requiere que las cosas que hereden de el tengan un contructor por defecto sin parametros
/// </summary>
[System.Serializable]
public abstract class LBSCharacteristic : ICloneable
{
    #region FIELDS
    [HideInInspector, JsonIgnore]
    private Bundle owner;

    [JsonRequired, SerializeField]
    protected string label = "";

    //[JsonIgnore, SerializeField]
    //private bool isSavableOnJSON = true;
    #endregion

    #region PROPERTIES
    [HideInInspector, JsonIgnore]
    public Bundle Owner => owner;

    [JsonIgnore]
    public string Label
    {
        get => label;
        set => label = value;
    }
    #endregion

    #region CONSTRUCTORS
    public LBSCharacteristic() { }

    public LBSCharacteristic(string label)
    {
        this.label = label;
    }
    #endregion

    #region METHODS
    /// <summary>
    /// esta funcion es para que la characteristic tenga axeso a el bundle que lo posee
    /// asi podemos tener acciones o itenracciones dentro bharacteristics
    /// </summary>
    public void Init(Bundle owner)
    {
        this.owner = owner;
        OnEnable();
    }

    public virtual void OnEnable() { }

    public abstract object Clone();


    public abstract override bool Equals(object obj);

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion

}
