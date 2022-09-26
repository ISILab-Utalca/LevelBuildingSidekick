using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Windows
{
    public class MapEliteWindow : EditorWindow
    {
        [MenuItem("ISILab/LBS plugin/MapEliteWindow", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<MapEliteWindow>();
            window.titleContent = new GUIContent("Map Elite");
        }

        public Button btn;
        public Vector2Field partitions;
        public VisualElement content;
        public DropdownField evaluation1;
        public DropdownField evaluation2;
        public ObjectField fieldIA;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteUXML");
            visualTree.CloneTree(root);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            root.styleSheets.Add(styleSheet);

            this.content = root.Q<VisualElement>("Content");
            this.evaluation1 = root.Q<DropdownField>("Evaluation1");
            this.evaluation2 = root.Q<DropdownField>("Evaluation2");
            this.fieldIA = root.Q<ObjectField>("IAField");
            this.btn = root.Q<Button>("Calculate");
            this.partitions = root.Q<Vector2Field>();

            Debug.Log(btn);
            btn.clicked += Calculate;
        }

        public void Calculate()
        {
            Debug.Log("SAA");
            content.Clear();
            if(partitions.value.x <= 0 || partitions.value.y <= 0)
            {
                Debug.Log("particiones en 0 , tienen que ser mayor a 1 por lo menos.");
                return;
            }

            for (int i = 0; i < partitions.value.x; i++)
            {
                for (int j = 0; j < partitions.value.y; j++)
                {
                    content.Add(new MapEliteElementView());
                }
            }
        }
    }
}