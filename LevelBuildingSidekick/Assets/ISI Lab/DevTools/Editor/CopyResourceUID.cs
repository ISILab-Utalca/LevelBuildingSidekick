using UnityEditor;
using UnityEngine;

namespace ISI_Lab.DevTools.Editor
{
    public class CopyResourceUID : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Copy Resource UID", false, 19)]
        private static void CopyUIDToClipboard()
        {
           Object obj = Selection.activeObject;
           if (obj != null)
           {
               string path = AssetDatabase.GetAssetPath(obj);
               string uid = AssetDatabase.AssetPathToGUID(path);
               GUIUtility.systemCopyBuffer = uid;
               Debug.Log("The GUID was copy to the clipboard:\n" + uid);
           }
        }
#endif
    }
}
