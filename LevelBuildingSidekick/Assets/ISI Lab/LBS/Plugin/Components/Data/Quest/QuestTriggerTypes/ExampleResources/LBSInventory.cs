using System;
using System.Collections.Generic;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Macros;
using UnityEngine;

[Serializable]
public class LBSInventory : MonoBehaviour
{
    [SerializeField]
    private readonly Dictionary<string, int> _inventory = new();

    /// <summary>
    /// Returns the amount of a given GUID, use after HasType
    /// </summary>
    public int GetTypeAmount(string guid)
    {
        return _inventory.GetValueOrDefault(guid, 0);
    }

    /// <summary>
    /// Returns true if a GUID is in the inventory
    /// </summary>
    public bool HasType(string guid)
    {
        return _inventory.ContainsKey(guid);
    }

    /// <summary>
    /// Call to add a GUID by n amount
    /// </summary>
    public void AddItems(string guid, int amount)
    {
        if (!_inventory.TryAdd(guid, amount))
            _inventory[guid] += amount;

        Debug.Log($"[LBSInventory] Added GUID: {guid}, New count: {_inventory[guid]}");
    }

    private void OnTriggerEnter(Collider other)
    {
        var lbsGen = other.gameObject.GetComponent<LBSGenerated>();
        if (lbsGen == null || lbsGen.BundleRef == null) return;

        string guid = LBSAssetMacro.GetGuidFromAsset(lbsGen.BundleRef);

        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogWarning("[LBSInventory] Skipped object with missing BundleRef GUID.");
            return;
        }

        AddItems(guid, 1);
        Destroy(other.gameObject);
    }
}
