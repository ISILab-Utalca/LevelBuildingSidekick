using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using LBS.Bundles;
using UnityEngine;

namespace ISILab.LBS
{
    [CreateAssetMenu(fileName = "New Tags Group", menuName = "ISILab/New Tags Group")]
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
        private List<LBSTag> tags = new List<LBSTag>();
        [SerializeField]
        public TagType type;
        #endregion

        #region PROPERTIES
        public List<LBSTag> Tags
        {
            get => new List<LBSTag>(tags);
        }
        #endregion

        #region METHODS
        public void RemoveAt(int index)
        {
            tags.RemoveAt(index);
        }

        public void Remove(LBSTag tag)
        {
            tags.Remove(tag);
        }

        public void Add(LBSTag tag)
        {
            tags.Add(tag);
        }
        #endregion
    }
}