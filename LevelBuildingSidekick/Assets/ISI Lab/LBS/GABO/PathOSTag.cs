using ISILab.Commons.Attributes;
using PathOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [CreateAssetMenu(fileName = "NewID", menuName = "ISILab/LBS/PathOS/PathOSTag")]
    [System.Serializable]
    public class PathOSTag : ScriptableObject
    {
        public enum PathOSCategory
        {
            ElementTag,
            EventTag
        }

        #region FIELDS
        [SerializeField, ReadOnly]
        private string label;
        [SerializeField]
        private PathOS.EntityType entityType;
        [SerializeField]
        protected Texture2D icon;
        [SerializeField]
        protected Color color;
        [SerializeField]
        protected PathOSCategory category;
        #endregion

        #region PROPERTIES
        public string Label
        {
            get => label;
            set
            {
                if (label == value)
                    return;

                this.label = value;
                OnChangeText?.Invoke(this);
            }
        }

        public EntityType EntityType
        {
            get => entityType;
            set
            {
                if (entityType == value)
                    return;

                this.entityType = value;
                OnChangeEntityType?.Invoke(this);
            }
        }

        public Texture2D Icon
        {
            get => icon;
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

        public PathOSCategory Category
        {
            get => category;
            set
            {
                if (category == value) return;
                category = value;
                OnChangeCategory?.Invoke(this);
            }
        }
        #endregion

        #region EVENTS
        public delegate void PathOSTagEvent(PathOSTag tag);
        public PathOSTagEvent OnChangeText;
        public PathOSTagEvent OnChangeEntityType;
        public PathOSTagEvent OnChangeColor;
        public PathOSTagEvent OnChangeIcon;
        public PathOSTagEvent OnChangeCategory;
        #endregion

        #region METHODS
        public void Init(string text, Color color, Texture2D icon)
        {
            this.label = text;
            this.color = color;
            this.icon = icon;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PathOSTag;
            if (other == null) return false;
            if (this.label != other.label) return false;
            if (this.color != other.color) return false;
            if (this.icon != other.icon) return false;
            if (this.category != other.category) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private void OnValidate()
        {
            this.label = this.name;
        }
        #endregion
    }
}
