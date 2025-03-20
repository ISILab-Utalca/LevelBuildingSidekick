using ISILab.Commons.Utility.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class KeyMapWindow : EditorWindow
    {
        [MenuItem("Window/ISILab/Hints controller", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<KeyMapWindow>();
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath("809c25c61768b1c41b1ed78f56c0d7da")); //Logo
            window.titleContent = new GUIContent("ToolTips", icon);
            window.minSize = new Vector2(350, 500);

        }

        public virtual void CreateGUI()
        {
            rootVisualElement.Add(new HintsController());

        }
    }
}
