using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utility
{
    public static class DirectoryTools
    {
        public static T SearchAssetByName<T>(string name)
        {
            //Debug.Log("Scanning for " + name + " of type " + typeof(T).ToString()); 
            var guids = AssetDatabase.FindAssets(name);
            //Debug.Log(guids.Length);
            object obj = null;
            foreach (var guid in guids)
            {
                obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T));
                if(obj != null)
                {
                    //Debug.Log(obj.GetType().ToString());
                    break;
                }
            }

            //Debug.Log("Type: " + obj.GetType().ToString()); 
            return (T)obj;
        } 
    }
}

