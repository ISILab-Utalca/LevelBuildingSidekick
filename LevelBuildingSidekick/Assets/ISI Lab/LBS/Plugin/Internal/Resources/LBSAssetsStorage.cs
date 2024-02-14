using ISILab.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ISILab.LBS.Internal
{
    [CreateAssetMenu(fileName = "New LBS Storage", menuName = "ISILab/LBS/Internal/AssetStorage")]
    public class LBSAssetsStorage : ScriptableObject
    {
        #region SUB GROUP
        [System.Serializable]
        public class TypeGroup
        {
            [SerializeField]
            public string type;
            [SerializeField]
            public List<string> items;

            public TypeGroup(Type type, List<string> items)
            {
                this.type = type.FullName;
                this.items = items;
            }
        }
        #endregion

        #region SINGLETON
        [System.NonSerialized]
        private static LBSAssetsStorage instance;

        public static LBSAssetsStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<LBSAssetsStorage>("Storage/LBS Storage");
                }
                return instance;
            }
        }
        #endregion

        #region FIELDS
        [SerializeField]
        private List<TypeGroup> groups = new List<TypeGroup>();
        #endregion

        #region METHODS
        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            instance = this;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        public List<T> Get<T>() where T : Object
        {
            var n = typeof(T).FullName;

            foreach (var group in groups)
            {
                if (group.type.Equals(typeof(T).FullName))
                {
                    var r = new List<T>();
                    foreach (var path in group.items)
                    {
                        var obj = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                        if (obj != null)
                            r.Add(obj);
                    }
                    return r;
                }
            }
            return null;
        }

        public List<ScriptableObject> Get(Type type)
        {
            foreach (var group in groups)
            {
                if (group.type.Equals(type.FullName))
                {
                    var r = new List<ScriptableObject>();
                    foreach (var path in group.items)
                    {
                        var obj = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                        //var obj = Resources.Load(path) as ScriptableObject;
                        if (obj != null)
                            r.Add(obj);
                    }
                    return r;
                }
            }
            return null;
        }

        public void RemoveElement(ScriptableObject obj)
        {
            foreach (var group in groups)
            {
                var path = AssetDatabase.GetAssetPath(obj);

                if (group.items.Remove(path))
                    return;
            }
        }

        public void AddElement(ScriptableObject obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);

            var type = obj.GetType();

            foreach (var group in groups)
            {
                if (group.type.Equals(type.FullName))
                {
                    if (!group.items.Contains(path))
                        group.items.Add(path);
                    return;
                }
            }
            groups.Add(new TypeGroup(type, new List<string>() { path }));
        }

        public void ClearGroup<T>()
        {
            foreach (var group in groups)
            {
                if (group.type.Equals(typeof(T).FullName))
                {
                    group.items.Clear();
                }
            }
        }

        public void CleanEmpties<T>()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                var current = groups[i];

                if (!current.type.Equals(typeof(T).FullName))
                    continue;

                current.items = current.items.Where(b => b != null).ToList();
            }
        }

        public void Clear()
        {
            groups.Clear();
        }

        public void CleanAllEmpties()
        {
            var toR = new List<TypeGroup>();
            for (int i = 0; i < groups.Count; i++)
            {
                var current = groups[i].items;
                current = current.Where(b => b != null).ToList();
                groups[i].items = current;

                if (groups[i].items.Count <= 0)
                    toR.Add(groups[i]);
            }

            toR.ForEach(t => groups.Remove(t));
        }

        #endregion
    }
}