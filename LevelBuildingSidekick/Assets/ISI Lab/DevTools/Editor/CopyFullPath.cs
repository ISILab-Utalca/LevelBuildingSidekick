using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Object = UnityEngine.Object;
namespace Nico.Tools

{
    public static class CopyFullPath
    {
#if UNITY_EDITOR
        
        [MenuItem("Assets/Copy File Absolute Path", false, 19)]
        private static void CopyPathToClipboard()
        {
            Object obj = Selection.activeObject;
            if (obj != null)
            {

                if (AssetDatabase.Contains(obj))
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    path = path.TrimStart('A', 's', 's', 'e', 't');

                    path = Application.dataPath + path;

                    path = path.Replace('/', '\\');

                    GUIUtility.systemCopyBuffer = path;

                    Debug.Log("The full path was copy to the clipboard:\n" + path);
                }
            }
        }
#endif
    }
}