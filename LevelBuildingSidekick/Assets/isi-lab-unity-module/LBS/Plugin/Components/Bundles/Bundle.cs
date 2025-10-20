using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using LBS.Bundles;
using LBS.Bundles.Tools;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

namespace LBS.Bundles
{
    [System.Flags]
    public enum BundleFlags
    {
        None = 0,
        Interior = 1 << 0,
        Exterior = 1 << 1,
        Population = 1 << 2,
    }
    
    [System.Serializable]
    public enum Positioning
    {
        Center,
        Edge,
        Corner
    }

    [System.Serializable]
    public class Asset : ICloneable
    {
        public GameObject obj;
        [Range(0f,1f)]
        public float probability;

        public Asset(GameObject obj, float probability)
        {
            this.obj = obj;
            this.probability = probability;
        }

        public object Clone()
        {
            return new Asset(this.obj, this.probability);
        }
    }

    //[CreateAssetMenu(fileName = "New Bundle", menuName = "ISILab/LBS/Bundle")] <- Replaced with BundleMenuItem
    [System.Serializable]
    public class Bundle : ScriptableObject, ICloneable
    {
        public Bundle()
        {
            layerContentFlags = BundleFlags.None;
        }

        public enum TagType
        {
            Aesthetic, // (Style)Ej: Castle, Spaceship,
            Structural, // Ej: Door, Wall, Corner,Stair
            Element, // Ej: Furniture, Enemies, 
                     // Distinction, // (characteristics)Ej: Destroyed, Blooded, Dirty,
        }
        
        [System.Flags]
        public enum EElementFlag
        {
            None      = 0,
            Character = 1 << 0, // player, npc, enemies
            Enemy     = 1 << 1 | Character, 
            Player    = 1 << 2 | Character,
            Ally      = 1 << 3 | Character,
            Item      = 1 << 4, // collectable type
            Resource  = 1 << 5 | Item,
            Equipment = 1 << 6 | Item,
            Interactable = 1 << 7, // buttons, doors, levers
            Trigger   = 1 << 8, // triggers 
            Prop = 1 << 9, // static mesh
            Misc = 1 << 10 // non categorized
        }

        #region FIELDS

        [FormerlySerializedAs("populationName")]
        [SerializeField]
        private string bundleName;

        // Add a flags field
        [FormerlySerializedAs("flags")]
        [SerializeField]

        private BundleFlags layerContentFlags;

        [SerializeField]
        private TagType type;

        [SerializeField]
        private Positioning anchorPosition = Positioning.Center;

        [SerializeField]
        private Color color;

        [SerializeField]
        private VectorImage icon;
        
        [SerializeField]
        private List<Asset> assets = new List<Asset>();

        [SerializeReference, HideInInspector]
        private List<LBSCharacteristic> characteristics = new List<LBSCharacteristic>();

        // only used if it's an element (population)
        [SerializeField,HideInInspector] 
        private EElementFlag elementFlag = EElementFlag.None;

        // Used in generation 3d.
        [SerializeField,HideInInspector] 
        private Vector2Int tileSize = Vector2Int.one;
        
        // hides in inspector and uses the custom GUI to assign only children with containing flags
        [SerializeField, HideInInspector]
        private List<Bundle> childsBundles = new List<Bundle>();
        
        [SerializeField]
        private MicroGenTool microGenTool = new MicroGenTool();

        [SerializeField, HideInInspector]
        private string guid;

        #endregion

        #region PROPERTIES

        public string BundleName
        {
            get => string.IsNullOrEmpty(bundleName) ? Name : bundleName;
            set => bundleName = value;
        }

        public BundleFlags LayerContentFlags
        {
            get => layerContentFlags;
            set => layerContentFlags = value;
        }

        public VectorImage Icon
        {
            get => !icon ? null : icon;
            set => icon = value;
        }
        public Color Color => color;
        public string Name => name;
        public List<Asset> Assets
        {
            get => new List<Asset>(assets);
            set => assets = value;
        }

        public Vector2Int TileSize => tileSize;
        
        public EElementFlag ElementFlag => elementFlag;
        public List<LBSCharacteristic> Characteristics => characteristics;

        public List<Bundle> ChildsBundles => new List<Bundle>(childsBundles);

        
        [SerializeField]
        public bool IsLeaf => (childsBundles.Count <= 0);

        public Positioning Positioning => anchorPosition;

        public TagType Type
        {
            get => type;
            set => type = value;
        }

        public string GUID
        {
            get => guid;
            set => guid = value;
        }

        #endregion

        #region EVENTS
        public event Action<Bundle> OnAddChild;
        public event Action<Bundle> OnRemoveChild;

        public event Action<Asset> OnAddAsset;
        public event Action<Asset> OnRemoveAsset;

        public event Action<LBSCharacteristic> OnAddCharacteristic;
        public event Action<LBSCharacteristic> OnRemoveCharacteristic;
        #endregion

        #region METHODS
        public List<Bundle> GetChildrenByPositioning(Positioning positioning)
        {
            var r = new List<Bundle>();
            foreach (var child in childsBundles)
            {
                if(child.anchorPosition == positioning)
                    r.Add(child);

                r.AddRange(child.GetChildrenByPositioning(positioning));
            }
            return r;
        }

        internal List<Bundle> GetChildrensByTag(string tag)
        {
            var r = new List<Bundle>();
            foreach (var child in childsBundles)
            {
                if (child.name == tag)
                    r.Add(child);

                r.AddRange(child.GetChildrensByTag(tag));
            }
            return r;
        }

        public void Reload()
        {
            foreach (var characteristic in characteristics)
            {
                if (characteristic != null)
                {
                    characteristic.Init(this);
                }
            }
        }

        /* Checks that a child to be added:
            - not in a child already
            - not parent of the current bundle
            - has at least one of the current bundle's flags
    
        */
        public bool IsBundleValidChild(Bundle potentialChild)
        {
            // Get all parent bundles to avoid recursion
            List<Bundle> parents = new List<Bundle>();
            var currentParent = this;
            while (currentParent != null)
            {
                parents.Add(currentParent);
                currentParent = currentParent.Parent();
            }
            if (!potentialChild.LayerContentFlags.HasFlag(LayerContentFlags)) return false;
            if (parents.Contains(potentialChild))  return false;
            if (ChildsBundles.Contains(potentialChild)) return false;
            return true;
        }
        
        
        public void AddChild(Bundle child)
        {
            if (IsRecursive(this, child))
            {
                Debug.Log("[ISI Lab]: Bundle '" +
                    this.name + "' is contained in bundle '" +
                    child.name + "' or one of its child bundles.");
                return;
            }

            if (!IsBundleValidChild(child)) return;

            childsBundles.Add(child);
            OnAddChild?.Invoke(child);
        }

        public void InsertChild(int index, Bundle child)
        {
            Assert.IsTrue(IsRecursive(this, child), "[ISI Lab]: Bundle '" + this.name + "' is contained in bundle '" + child.name + "' or one of its child bundles.");

            childsBundles.Insert(index, child);
            OnAddChild?.Invoke(child);
        }

        public void RemoveChild(Bundle child)
        {
            if (childsBundles.Remove(child))
            {
                OnRemoveChild?.Invoke(child);
            }
        }
        
        public void RemoveNullChildren()
        {
            for (int i = 0; i < childsBundles.Count; i++)
            {
                if (childsBundles[i] == null)
                {
                    childsBundles.RemoveAt(i);
                    i--;
                }
            }
        }

        public void ClearChilds()
        {
            while (childsBundles.Count() > 0)
            {
                var last = childsBundles.Last();
                OnRemoveChild?.Invoke(this);
                childsBundles.Remove(last);
            }
        }

        public void AddAsset(GameObject obj, float provability = .5f)
        {
            var asset = new Asset(obj, provability);
            assets.Add(asset);
            OnAddAsset?.Invoke(asset);
        }

        public void AddAsset(Asset asset)
        {
            assets.Add(asset);
            OnAddAsset?.Invoke(asset);
        }

        public void ReplaceAsset(int index, Asset asset)
        {
            if (index == -1)
                return;

            OnRemoveAsset?.Invoke(assets[index]);
            assets[index] = asset;
            OnAddAsset?.Invoke(asset);
        }

        public void InsertAsset(int index, Asset asset)
        {
            assets.Insert(index, asset);
            OnAddAsset?.Invoke(asset);
        }

        public void RemoveAsset(Asset asset)
        {
            if (assets.Remove(asset))
                OnRemoveAsset?.Invoke(asset);
        }

        public void AddCharacteristic(LBSCharacteristic characteristic)
        {
            characteristics.Add(characteristic);
            characteristic.Init(this);
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
            if (characteristics.Remove(characterictic))
            {
                OnRemoveCharacteristic?.Invoke(characterictic);
            }
        }

        public List<T> GetChildrenCharacteristics<T>() where T : LBSCharacteristic
        {
            var chars = new List<T>();

            chars.AddRange(GetCharacteristics<T>());

            foreach (var child in childsBundles)
            {
                if(child == null) continue;
                var subChars = child.GetChildrenCharacteristics<T>();
                chars.AddRange(subChars);
            }
            return chars;
        }

        public List<T> GetCharacteristics<T>() where T : LBSCharacteristic
        {
            var list = new List<T>();
            foreach (object item in characteristics)
            {
                if (item is T)
                {
                    list.Add((T)item);
                }
            }

            return list;
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
                var b = child.Clone() as Bundle;
                other.AddChild(b);
            }

            foreach (var asset in assets)
            {
                other.AddAsset(asset.Clone() as Asset);
            }

            other.color = this.color;
            other.icon = this.icon;

            return other;
        }
        
        /// <summary>
        /// true if the bundle has the label characteristic
        /// </summary>
        public bool GetHasTagCharacteristic(string label)
        {
            foreach (var c in Characteristics)
            {
                var tag = c as LBSTagsCharacteristic;
                if (tag != null && tag.Value != null && tag.Value.Label == label)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// true if the bundle has one of the labels
        /// </summary>
        public bool GetHasTagCharacteristic(List<string> labels)
        {
            if (labels == null || labels.Count == 0)
                return false;

            foreach (var c in Characteristics)
            {
                var tag = c as LBSTagsCharacteristic;
                if (tag != null && tag.Value != null && labels.Contains(tag.Value.Label))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// true if the Bundle has all the labels
        /// </summary>
        public bool GetHasAllTagCharacteristics(List<string> labels)
        {
            if (labels == null || labels.Count == 0)
                return false;

            // collect all characteristic labels into a HashSet
            HashSet<string> characteristicLabels = new HashSet<string>();
            foreach (var c in Characteristics)
            {
                var tag = c as LBSTagsCharacteristic;
                if (tag != null && tag.Value != null)
                    characteristicLabels.Add(tag.Value.Label);
            }

            // check that every label is present
            foreach (var label in labels)
            {
                if (!characteristicLabels.Contains(label))
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Returns true if the bundle has only the given flag (and no others).
        /// Example: if bundle is Enemy, query Enemy -> true.
        ///          if bundle is Enemy | Item, query Enemy -> false.
        /// </summary>
        public bool HasOnlyFlag(EElementFlag queryFlag)
        {
            return ElementFlag == queryFlag;
        }

        
        /// <summary>
        /// Returns true if the bundle has all of the given flags.
        /// Example: Enemy | Item returns true if queried with { Character, Item }.
        /// </summary>
        public bool HasAllFlags(HashSet<EElementFlag> queryFlags)
        {
            foreach (var flag in queryFlags)
            {
                if ((ElementFlag & flag) != flag)
                    return false;
            }
            return true;
        }

        
        /// <summary>
        /// Returns true if the bundle has at least one of the given flags
        /// or falls under its parent category.
        /// Example: Enemy will return true if checked against { Character, Player } because Enemy includes Character.
        /// </summary>
        public bool HasAnyFlag(HashSet<EElementFlag> queryFlags)
        {
            foreach (var flag in queryFlags)
            {
                if ((ElementFlag & flag) == flag)
                    return true;
            }

            return false;
        }

        
        public bool GetHasAnyTagCharacteristics(List<string> labels)
        {
            if (labels == null || labels.Count == 0)
                return false;

            // collect all characteristic labels into a HashSet
            HashSet<string> characteristicLabels = new HashSet<string>();
            foreach (var c in Characteristics)
            {
                var tag = c as LBSTagsCharacteristic;
                if (tag != null && tag.Value != null)
                    characteristicLabels.Add(tag.Value.Label);
            }

            // check that it has at least one
            foreach (var label in labels)
            {
                if (characteristicLabels.Contains(label))
                    return true;
            }

            return false;
        }

        public MicroGenTool GetMicroGenTool()
        {
            return microGenTool;
        }

        public void ClearEvents()
        {
            OnAddChild = null;
            OnRemoveChild = null;
        }
        #endregion

        #region STATIC FUNCTIONS

        public static bool IsRecursive(Bundle parent, Bundle child) // mover a extensions (!)
        {
            if (parent == child) return true;
            if (child.ChildsBundles.Contains(parent)) return true;
            
            foreach (var ch in child.ChildsBundles)
            {
                if (IsRecursive(parent, ch))
                {
                    return true;
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
            var storage = LBSAssetsStorage.Instance;

            var x = storage.Get<Bundle>().ToList();
            var xx = x.Where(b => b.ChildsBundles.Contains(bundle)).ToList();
            var b = xx.Count() <= 0;
            return b;
        }

        public static Bundle Parent(this Bundle bundle)
        {
            var parent = LBSAssetsStorage.Instance.Get<Bundle>()
                .Find(b => b.ChildsBundles.Contains(bundle));

            return parent;
        }
        
        public static List<Bundle> Parents(this Bundle bundle)
        {
            var parents = LBSAssetsStorage.Instance.Get<Bundle>()
                .FindAll(b => b.ChildsBundles.Contains(bundle));

            return parents;
        }
    }
    
}
