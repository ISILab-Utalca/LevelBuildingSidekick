using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Bundle : LBSScriptableObject
{

    [SerializeField]
    protected LBSIdentifier id;

    public LBSIdentifier ID
    {
        get => id;
        set => id = value;
    }

    /*[SerializeField,*/[ SerializeReference, HideInInspector]
    protected List<LBSCharacteristic> characteristics;

    public abstract void Add(List<Bundle> data);
    public abstract LBSCharacteristic GetTag(int index);
    public abstract List<LBSCharacteristic> GetCharacteristics();
    public abstract GameObject GetObject(int index);
    public abstract List<GameObject> GetObjects(List<string> tags = null);
    public abstract void Remove(List<Bundle> data);
}

public abstract class LBSScriptableObject : ScriptableObject
{ 
}