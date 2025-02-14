using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace LBS.Bundles
{

    public enum PopulationType
    {
        Entity, // player, npc, enemies
        Object, // collectable type
        Interactable, // buttons, doors, levers
        Area, // triggers 
        Prop // static mesh
    }
    
    [System.Flags]
    public enum BundleFlags
    {
        None = 0,
        Interior = 1 << 0,
        Exterior = 1 << 1,
        Population = 1 << 2,
        //NewOne = 1 << 3,
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

    [CreateAssetMenu(fileName = "New Bundle", menuName = "ISILab/LBS/Bundle")]
    [System.Serializable]
    public class Bundle : ScriptableObject, ICloneable
    {

        public Bundle()
        {
            layerContentFlags = BundleFlags.None;
        }
        
        // Add a flags field
        [FormerlySerializedAs("flags")] [SerializeField]
        private BundleFlags layerContentFlags;

        public BundleFlags LayerContentFlags => layerContentFlags;
        
        public enum TagType
        {
            Aesthetic, // (Style)Ej: Castle, Spaceship,
            Structural, // Ej: Door, Wall, Corner,Stair
            Element, // Ej: Furniture, Enemies, 
                     // Distinction, // (characteristics)Ej: Destroyed, Blooded, Dirty,
        }
        #region FIELDS
        [SerializeField]
        private TagType type;

        [SerializeField]
        private Positioning anchorPosition = Positioning.Center;

        [SerializeField]
        private Color color;

        [SerializeField]
        private Texture2D icon;
        
        [SerializeField]
        private List<Asset> assets = new List<Asset>();

        [SerializeReference, HideInInspector]
        private List<LBSCharacteristic> characteristics = new List<LBSCharacteristic>();

        // only used if it's an element (population)
        [SerializeField,HideInInspector] 
        private PopulationType populationType = PopulationType.Entity;

        // Used in generation 3d.
        [SerializeField,HideInInspector] 
        private Vector2Int tileSize = Vector2Int.one;
        
        // hides in inspector and uses the custom GUI to assign only children with containing flags
        [SerializeField, HideInInspector]
        private List<Bundle> childsBundles = new List<Bundle>();

        #endregion

        #region PROPERTIES
        public Texture2D Icon => icon;
        public Color Color => color;
        public string Name => name;
        public List<Asset> Assets
        {
            get => new List<Asset>(assets);
            set => assets = value;
        }

        public Vector2Int TileSize => tileSize;
        
        public PopulationType PopulationType => populationType;
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
    }
    
}
