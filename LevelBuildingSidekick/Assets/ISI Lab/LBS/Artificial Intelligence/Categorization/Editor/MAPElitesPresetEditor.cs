using ISILab.LBS.AI.Categorization;
using ISILab.LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [CustomEditor(typeof(MAPElitesPreset))]
    public class MAPElitesPresetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new MAPElitesPresetVE(target);

            root.TrackSerializedObjectValue(serializedObject, Save);

            return root;
        }

        public void Save(SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedProperties();
            var preset = serializedObject.targetObject as MAPElitesPreset;
            EditorUtility.SetDirty(preset);
        }
    }
}