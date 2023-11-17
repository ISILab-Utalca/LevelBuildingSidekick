using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "New LBS Storage",menuName = "ISILab/LBS/Internal/AssetStorage")]
public class LBSAssetsStorage : ScriptableObject
{
    #region SUB GROUP
    [System.Serializable]
    public class TypeGroup
    {
        [SerializeField]
        public string type;
        [SerializeField]
        public List<ScriptableObject> items;

        public TypeGroup(Type type, List<ScriptableObject> items)
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
            if(instance == null)
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
        //CleanAllEmpties();
        foreach (var group in groups)
        {
            if(group.type.Equals(typeof(T).FullName))
            {
                return new List<T>(group.items.Cast<T>());
            }
        }
        return null;
    }

    public List<ScriptableObject> Get(Type type)
    {
        //CleanAllEmpties();
        foreach (var group in groups)
        {
            if (group.type.Equals(type.FullName))
            {
                return new List<ScriptableObject>(group.items);
            }
        }
        return null;
    }

    public void RemoveElement(ScriptableObject obj)
    {
        foreach (var group in groups)
        {
            if(group.items.Remove(obj))
            {
                return;
            }
        }
    }

    public void AddElement(ScriptableObject obj)
    {
        var type = obj.GetType();

        foreach (var group in groups)
        {
            if(group.type.Equals(type.FullName))
            {
                if(!group.items.Contains(obj))
                    group.items.Add(obj);
                return;
            }
        }
        groups.Add(new TypeGroup(type, new List<ScriptableObject>() { obj }));
    }

    public void ClearGroup<T>()
    {
        foreach (var group in groups)
        {
            if(group.type.Equals(typeof(T).FullName))
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