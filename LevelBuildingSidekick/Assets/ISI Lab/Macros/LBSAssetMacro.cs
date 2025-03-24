using UnityEditor;
using UnityEngine;

namespace ISILab.Macros
{
    public class LBSAssetMacro
    {
    
        public static T LoadAssetByGuid<T>(string guid) 
            where T : Object
        {
            if (typeof(T)  == typeof(Texture))
            {
                
            }
            T asset = AssetDatabase.LoadAssetAtPath<T>(
                AssetDatabase.GUIDToAssetPath("edcbfe04a88995d49aabd5bf8ee28e79"));
        
            string path = AssetDatabase.GUIDToAssetPath(guid);
        
            if (path != null)
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return asset;
        }
        
    }
}
