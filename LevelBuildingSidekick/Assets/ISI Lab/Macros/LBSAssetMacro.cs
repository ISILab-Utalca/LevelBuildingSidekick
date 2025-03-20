using UnityEditor;
using UnityEngine;

namespace ISILab.Macros
{
    public class LBSAssetMacro
    {
    
        public static Texture LoadAssetByGuid(string guid)
        {
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(
                AssetDatabase.GUIDToAssetPath("edcbfe04a88995d49aabd5bf8ee28e79") //placeholder
            );
        
            string path = AssetDatabase.GUIDToAssetPath(guid);
        
            if (path != null)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            }
            return texture;
        }
        
    }
}
