using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.MapTools.Editor
{
    [CustomEditor(typeof(LBSGenerated))]   
    public class LBSGeneratedEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LBSGenerated targetLbs = (LBSGenerated)target;
            DrawDefaultInspector();

            if (targetLbs.Spread == LBSGenerated.SpreadType.Center)
            {
                EditorGUILayout.LabelField("Hola");
                Bounds b = new Bounds();
                EditorGUILayout.BoundsField(b);
            }
        }
    }
}
