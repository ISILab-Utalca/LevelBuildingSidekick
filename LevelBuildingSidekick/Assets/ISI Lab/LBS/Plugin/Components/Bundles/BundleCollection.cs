using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LBS.Bundles
{

    [CreateAssetMenu(fileName = "New Bundle Collection", menuName = "ISILab/LBS/BundleCollection")]
    [System.Serializable]
    public class BundleCollection : ScriptableObject, ICloneable
    {

        public BundleCollection()
        {

        }

        #region FIELDS

        [FormerlySerializedAs("type")] [SerializeField]
        private Bundle.TagType collectionType;

        [SerializeField] private Color color;

        [SerializeField] private Texture2D icon;

        [SerializeField] private List<Bundle> collection = new List<Bundle>();

        #endregion

        #region PROPERTIES

        public Texture2D Icon => icon;
        public Color Color => color;
        public string Name => name;
        public List<Bundle> Collection => collection;

        #endregion

        #region METHODS
  
        // removes all the bundles that are different from collection-type
        public void CleanCollection()
        {
            collection.RemoveAll(b => b.Type != (Bundle.TagType)collectionType);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
