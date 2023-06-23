using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEditor;

namespace UnityEngine.UIElements
{
    public class UxmlObjectFieldAttributeDescription : TypedUxmlAttributeDescription<Object>
    {
        ObjectField objectField = new ObjectField();

        public UxmlObjectFieldAttributeDescription()
            : base() { }

        public UxmlObjectFieldAttributeDescription(string attributeName, string propertyName, string defaultValue = null)
            : base()
        {
            
        }

        public override Object GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
        {
            string path = bag.TryGetAttributeValue("icon", out string val) ? val : null;
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<Object>(path);
            }

            return null;
        }
    }
}
