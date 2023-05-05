using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bundle", menuName = "ISILab/Bundle (*)")]
[System.Serializable]
public class Bundle : ScriptableObject
{
    #region FIELDS
    public bool isPreset = false;

    [SerializeField]
    private LBSIdentifier id;

    [SerializeField]
    private List<Bundle> childsBundles = new List<Bundle>();

    [SerializeField]
    private List<GameObject> assets = new List<GameObject>();

    [SerializeField, SerializeReference]
    private List<LBSCharacteristic> characteristics = new List<LBSCharacteristic>();
    #endregion

    #region EVENTS
    public event Action<Bundle> OnAddChild;
    public event Action<Bundle> OnRemoveChild;

    public event Action<GameObject> OnAddAsset;
    public event Action<GameObject> OnRemoveAsset;

    public event Action<LBSCharacteristic> OnAddCharacteristic;
    public event Action<LBSCharacteristic> OnRemoveCharacteristic;
    #endregion

    #region PROPERTIES
    public LBSIdentifier ID
    {
        get => id;
        set => id = value;
    }

    public List<Bundle> ChildsBundles => new List<Bundle>(childsBundles);

    public List<GameObject> Assets => new List<GameObject>(assets);

    public List<LBSCharacteristic> Characteristics => new List<LBSCharacteristic>(characteristics);

    [SerializeField]
    public bool IsLeaf => childsBundles.Count <= 0;
    #endregion

    #region METHODS
    public void AddChild(Bundle child)
    {
        childsBundles.Add(child);
        OnAddChild?.Invoke(child);
    }

    public void InsertChild(int index,Bundle child)
    {
        childsBundles.Insert(index, child);
        OnAddChild?.Invoke(child);
    }

    public void RemoveChild(Bundle child)
    {
        if(childsBundles.Remove(child))
        {
            OnRemoveChild?.Invoke(child);
        }
    }

    public void ClearChilds()
    {
        while(childsBundles.Count() > 0)
        {
            var last = childsBundles.Last();
            OnRemoveChild?.Invoke(this);
            childsBundles.Remove(last);
        }
    }

    public void AddAsset(GameObject asset)
    {
        assets.Add(asset);
        OnAddAsset?.Invoke(asset);
    }

    public void InsertAsset(int index, GameObject asset)
    {
        assets.Insert(index, asset);
        OnAddAsset?.Invoke(asset);
    }

    public void RemoveAsset(GameObject asset)
    {
        if(assets.Remove(asset))
            OnRemoveAsset?.Invoke(asset);
    }

    public void AddCharacteristic(LBSCharacteristic characteristic)
    {
        characteristic.Init(this);
        characteristics.Add(characteristic);
        OnAddCharacteristic?.Invoke(characteristic);
    }

    public void InsertAsset(int index, LBSCharacteristic characteristic)
    {
        characteristic.Init(this);
        characteristics.Insert(index, characteristic);
        OnAddCharacteristic?.Invoke(characteristic);
    }

    public void RemoveCharacteristic(LBSCharacteristic characterictic)
    {
        if(characteristics.Remove(characterictic))
        {
            OnRemoveCharacteristic?.Invoke(characterictic);
        }
    }

    public T GetCharacteristic<T>()
    {
        foreach (object item in characteristics)
        {
            if (item is T)
            {
                return (T)item;
            }
        }

        return default(T);
    }
    #endregion

}

