using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LBS.Bundles;

//[CreateAssetMenu(fileName = "NewIDBundle", menuName = "ISILab/Identifiers Bundle")]
public class LBSIdentifierBundle : ScriptableObject
{
    public enum TagType
    {
        Aesthetic, // (Style)Ej: Castle, Spaceship,
        Structural, // Ej: Door, Wall, Corner,Stair
        Element, // Ej: Furniture, Enemies, 
        // Distinction, // (characteristics)Ej: Destroyed, Blooded, Dirty,
    }

    #region FIELDS
    [SerializeField]
    private List<LBSIdentifier> tags = new List<LBSIdentifier>();
    [SerializeField]
    public TagType type;
    #endregion

    #region PROPERTIES
    public List<LBSIdentifier> Tags
    {
        get => new List<LBSIdentifier>(tags);
    }
    #endregion

    #region METHODS
    public void RemoveAt(int index)
    {
        tags.RemoveAt(index);
    }

    public void Remove (LBSIdentifier tag)
    {
        tags.Remove(tag);
    }

    public void Add(LBSIdentifier tag)
    {
        tags.Add(tag);
    }

    /*
    public void Add(List<Bundle> data) // (?) por que existe este metodo?
    {
        var names = data.Select(b => b.name);

        foreach (var name in names)
        {
            if(!this.tags.Contains(name))
                this.tags.Add(name);
        }
    }

    public virtual void Remove(List<Bundle> data) // (?) por que existe este metodo?
    {
        var tags = data.Select(b => b.ID);

        foreach(var tag in tags)
        {
            this.tags.Remove(tag);
        }    
    }
    */
    #endregion
}
