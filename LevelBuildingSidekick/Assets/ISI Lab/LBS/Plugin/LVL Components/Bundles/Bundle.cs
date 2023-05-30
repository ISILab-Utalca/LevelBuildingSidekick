using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "New Bundle", menuName = "ISILab/Bundle (*)")]
[System.Serializable]
public class Bundle : ScriptableObject, ICloneable
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

    public List<GameObject> Assets
    {
        get => new List<GameObject>(assets);
        set => assets = value;
    }

    public List<LBSCharacteristic> Characteristics => new List<LBSCharacteristic>(characteristics);

    [SerializeField]
    public bool IsLeaf => childsBundles.Count <= 0;
    #endregion

    #region METHODS
    public void AddChild(Bundle child)
    {
        if(IsRecursive(this, child))
        {
            Debug.Log("[ISI Lab]: Bundle '" +
                this.name + "' is contained in bundle '" +
                child.name + "' or one of its child bundles.");
            return;
        }

        childsBundles.Add(child);
        OnAddChild?.Invoke(child);
    }

    public void InsertChild(int index,Bundle child)
    {
        Assert.IsTrue(IsRecursive(this, child), "[ISI Lab]: Bundle '" + this.name + "' is contained in bundle '" + child.name + "' or one of its child bundles.");

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

    public void ReplaceAsset(int index, GameObject asset)
    {
        if (index == -1)
            return;

        OnRemoveAsset?.Invoke(assets[index]);
        assets[index] = asset;
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

    public void InsertCharacteristic(int index, LBSCharacteristic characteristic)
    {
        characteristic.Init(this);
        characteristics.Insert(index, characteristic);
        OnAddCharacteristic?.Invoke(characteristic);
    }

    public void RemoveAssetAt(int index)
    {
        var asset = assets[index];
        assets.RemoveAt(index);
        OnRemoveAsset?.Invoke(asset);
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

    public object Clone()
    {
        var other = ScriptableObject.CreateInstance<Bundle>();

        foreach (var charc in this.characteristics)
        {
            other.AddCharacteristic(charc.Clone() as LBSCharacteristic);
        }

        foreach (var child in this.childsBundles)
        {
            var b = child.Clone() as Bundle; // (!) esto puede causar una recursion
            other.AddChild(b);
        }

        foreach (var asset in assets)
        {
            other.AddAsset(asset);
        }

        other.ID = this.id;

        return other;
    }

    #endregion

    #region STATIC FUNCTIONS
    private static bool IsRecursive(Bundle parent, Bundle child) // mover a extensions (!)
    {
        if (parent == child)
            return true;

        if (child.ChildsBundles.Contains(parent))
        {
            return true;
        }
        else
        {
            foreach (var ch in child.ChildsBundles)
            {
                if (IsRecursive(parent, child))
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion
}

public static class BundleExtensions
{
    public static bool IsRoot(this Bundle bundle)
    {
        var x = Utility.DirectoryTools.GetScriptables<Bundle>().ToList();
        var xx = x.Where(b => b.ChildsBundles.Contains(bundle)).ToList();
        var b = xx.Count() <= 0;
        return b;
    }

    public static Bundle Parent(this Bundle bundle)
    {
        var parent = Utility.DirectoryTools.GetScriptables<Bundle>()
            .Find(b => b.ChildsBundles.Contains(bundle));

        return parent;
    }
}

