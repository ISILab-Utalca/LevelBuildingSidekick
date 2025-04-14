using UnityEditor;
using UnityEngine;

namespace ISILab.Macros
{
    public class LBSAssetMacro
    {
        public static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return !string.IsNullOrEmpty(path) ? AssetDatabase.LoadAssetAtPath<T>(path) : null;
        }

    }
}
