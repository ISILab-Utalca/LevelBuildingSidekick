// LBSInventory.cs
using System;
using System.Collections.Generic;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Macros;
using LBS.Bundles;
using UnityEngine;

[Serializable]
public class LBSInventory : MonoBehaviour
{
    // Delegate for item added event
    public delegate void ItemAddedDelegate(string guid, int amount);
    public event ItemAddedDelegate OnItemAdded;
    
    public readonly Dictionary<string, int> Inventory = new();

    /// <summary>
    /// Returns the amount of a given GUID, use after HasType
    /// </summary>
    public int GetTypeAmount(string guid)
    {
        return Inventory.GetValueOrDefault(guid, 0);
    }

    /// <summary>
    /// Returns true if a GUID is in the inventory
    /// </summary>
    public bool HasType(string guid)
    {
        return Inventory.ContainsKey(guid);
    }

    /// <summary>
    /// Call to add a GUID by n amount
    /// </summary>
    public void AddItems(string guid, int amount)
    {
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogWarning("[LBSInventory] Attempted to add item with empty GUID.");
            return;
        }

        if (!Inventory.TryAdd(guid, amount))
            Inventory[guid] += amount;

        Debug.Log($"[LBSInventory] Added GUID: {guid}, New count: {Inventory[guid]}");
        OnItemAdded?.Invoke(guid, amount);
    }

    private void OnTriggerEnter(Collider other)
    {
        var lbsGen = other.gameObject.GetComponent<LBSGenerated>();
        if (lbsGen == null || lbsGen.BundleRef == null) return;

        // Can only equip items
        if (lbsGen.BundleRef.PopulationType == Bundle.PopulationTypeE.Item)
        {
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
}