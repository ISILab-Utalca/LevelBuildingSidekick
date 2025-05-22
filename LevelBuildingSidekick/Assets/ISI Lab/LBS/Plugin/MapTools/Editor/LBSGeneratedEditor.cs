using System;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using LBS.Bundles;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ISI_Lab.LBS.Plugin.MapTools.Editor
{
    [CustomEditor(typeof(LBSGenerated))]   
    public class LBSGeneratedEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LBSGenerated LBSgen = (LBSGenerated)target;

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Original Bundle", LBSgen.BundleRef, typeof(Bundle), false);
            EditorGUILayout.ObjectField("Temporal Bundle", LBSgen.BundleTemp, typeof(Bundle), false);
        }
    }
}
