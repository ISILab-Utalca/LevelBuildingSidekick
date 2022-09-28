using LBS.Generator;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;
using System.Reflection;

namespace LBS
{

    [System.Serializable]
    public class ItemCategory
    {
        [SerializeField,JsonRequired]
        [StringEnum]
        public string category;


        [SerializeField, JsonRequired]
        public PivotType pivotType;

        [SerializeField, JsonIgnore]
        public List<GameObject> items = new List<GameObject>(); // cambiar a string (??)
        private List<string> itemNames = new List<string>();

        [JsonIgnore]
        public List<string> UpdatedItemNames
        {
            get
            {
                if(items != null)
                {
                    if(items.Count == 0)
                    {
                        itemNames = new List<string>();
                    }
                    else
                    {
                        itemNames = items.Select(go => go.name).ToList();
                    }
                }
                return itemNames;
            }
            set
            {
                itemNames = value;
            }
        }
        [JsonIgnore]
        public List<string> ItemNames
        {
            get
            {
                return itemNames;
            }
        }

        public ItemCategory(string _category, List<GameObject> _items)
        {
            category = _category;
            items = _items;
        }

        public ItemCategory(string name)
        {
            category = name;
            items = new List<GameObject>();
        }

        public ItemCategory()  //(?)
        {
            items = new List<GameObject>();
        }

    }

    public class StringEnumAttribute : PropertyAttribute
    {
        public Tags_SO DB;

        public StringEnumAttribute()
        {
            DB = DirectoryTools.GetScriptable<Tags_SO>();
        }
    }

    [CustomPropertyDrawer(typeof(StringEnumAttribute))]
    public class SEDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = attribute as StringEnumAttribute;

            var all = new List<string>(att.DB.Basics).Concat(att.DB.Others).ToList();

            var db = all.Append("Add new...");

            var n = all.IndexOf(property.stringValue);
            var t = EditorGUI.Popup(position, n, db.ToArray());
            property.stringValue = all.ToList()[t];

        }
    }

}
