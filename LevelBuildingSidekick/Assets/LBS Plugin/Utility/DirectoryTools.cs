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
            var guids = AssetDatabase.FindAssets(name);
            object obj = null;
            foreach (var guid in guids)
            {
                obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(object));
                if(obj != null && obj is T)
                {
                    break;
                }
            }

            return (T)obj;
        }
    }
}

