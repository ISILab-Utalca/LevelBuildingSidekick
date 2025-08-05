using ISILab.Commons.Attributes;
using ISILab.Macros;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PathOS;

namespace ISILab.LBS.Components
{
    [CreateAssetMenu(fileName = "NewID", menuName = "ISILab/LBS/Tag")]
    [System.Serializable]
    public class LBSTag : ScriptableObject
    {
        #region FIELDS
        [ReadOnly]
        public string label;
        [SerializeField]
        protected VectorImage icon;
        [SerializeField]
        protected Color color;
        [Header("Simulation")]
        [Space]
        [SerializeField]
        protected EntityType defaultType = EntityType.ET_NONE;
        [SerializeField]
        protected List<EntityType> admissibleTypes = new List<EntityType>();
        #endregion

        #region PROPERTIES
        public string Label
        {
            get => label;
            set
            {
                if (label == value) return;
                label = value;
                OnChangeText?.Invoke(this);
            }
        }

        public VectorImage Icon
        {
            get
            {
                if (icon == null)
                {
                    return LBSAssetMacro.LoadAssetByGuid<VectorImage>("d6f94a68988be8b45894b9f0e677e8d1");
                }
                return icon;
            }
            set
            {
                if (icon == value)
                    return;

                icon = value;
                OnChangeIcon?.Invoke(this);
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                if (color == value)
                    return;

                color = value;
                OnChangeColor?.Invoke(this);
            }
        }

        public EntityType EntityType
        {
            get => defaultType;
            set
            {
                if(defaultType == value) return;

                defaultType = value;
                OnChangeEntityType?.Invoke(this);
            }
        }

        public List<EntityType> AdmissibleEntityTypes { get => admissibleTypes; }
        #endregion

        #region EVENTS
        public delegate void TagEvent(LBSTag tag);
        public TagEvent OnChangeText;
        public TagEvent OnChangeColor;
        public TagEvent OnChangeIcon;
        public TagEvent OnChangeEntityType;
        #endregion

        #region METHODS
        public void Init(string text, Color color, VectorImage icon)
        {
            this.label = text;
            this.color = color;
            this.icon = icon;
        }

        public override bool Equals(object other)
        {
            if (other is not LBSTag obj) return false;
            //if(other == null) return false;
            //var obj = other as LBSTag;
            //if(obj == null) return false;

            return Equals(label, obj.Label);
                //&& icon .Equals(obj.icon)
                //&& color.Equals(obj.color);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private void OnValidate()
        {
            label = name;
        }
        #endregion
    }
}

