using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

namespace LBS.Windows
{
    public class BrushWindow : EditorWindow, INameable // o StampWindow
    {
        public static StampPresset SelectedStamp;


        [MenuItem("ISILab/LBS plugin/Brush window", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<BrushWindow>();
            window.titleContent = new GUIContent(window.GetName()); // o stamp window
        }

        private BrushView preview;

        private MaskField flags;
        private Button addBrushButton;
        private VisualElement brushesPanel;

        private VisualElement varPanel;
        private Button addVarButton;

        private TextField nameField;
        private DropdownField typeField;
        private SliderInt sizeSlider;
        private IntegerField sizeField;

        private StampPresset current;

        private LBSTags tags;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BrushWindowUXML");
            visualTree.CloneTree(root);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("BrushWindowUSS");
            root.styleSheets.Add(styleSheet);

            preview = root.Q<BrushView>("Preview");

            flags = root.Q<MaskField>();
            addBrushButton = root.Q<Button>("AddBrush");
            brushesPanel = root.Q<VisualElement>("BrushPanel");

            varPanel = root.Q<VisualElement>("VarPanel");
            addVarButton = root.Q<Button>("AddVar");

            nameField = root.Q<TextField>();
            typeField = root.Q<DropdownField>();
            sizeSlider = root.Q<SliderInt>();
            sizeField = root.Q<IntegerField>();

            sizeSlider.highValue = 100;
            sizeSlider.lowValue = 100;
            sizeSlider.value = 1;

            sizeSlider.RegisterValueChangedCallback(v => ChangeSize(v.newValue));
            addBrushButton.clicked += NewBrush;

            this.tags = LBSTags.GetInstance("Brush tags");
            flags.choices = tags.Alls;
            flags.RegisterValueChangedCallback(v => ActualizeBrushPanel(LayerToTag(v.newValue)));
            ActualizeBrushPanel(tags.Alls);
            Select(DirectoryTools.GetScriptable<StampPresset>());
        }

        private List<string> LayerToTag(int map)
        {
            var r = new List<string>();
            for (int i = 0; i < tags.Alls.Count; i++)
            {
                int layer = 1 << i;
                if (layer != 0)
                {
                    r.Add(tags.Alls[i]);
                }
            }
            return r;
        }

        // Este metodo aun no incluye la funcionalidade disernir
        // que brushes mostrar segun sus tag, ahora muestra todas (!)
        private void ActualizeBrushPanel(List<string> tags)
        {
            var stamps = DirectoryTools.GetScriptables<StampPresset>().ToList();
            ClearBrushPanel();

            foreach (var stamp in stamps)
            {
                AddBrush(stamp);
            }
        }

        private void Select(StampPresset presset)
        {
            preview.SetValue(presset);
            SelectedStamp = presset;
        }

        private void NewBrush()
        {
            var path = "Assets" + EditorUtility.SaveFilePanel("Add new Brush","","","asset").Split("Assets")[1];
            var asset = ScriptableObject.CreateInstance<StampPresset>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            AddBrush(asset);
        }

        private void AddBrush(StampPresset presset)
        {
            var brush = new BrushView(presset);
            brush.style.height = brush.style.width = 50;
            brush.clicked += () => { Select(brush.Presset); };
            brushesPanel.Add(brush);
        }

        private void ClearBrushPanel()
        {
            brushesPanel.Clear();
        }

        private void ChangeSize(int v)
        {
            //current.sizeBrush = v;
            sizeField.value = v;
        }

        public string GetName()
        {
            return "Brush Window";
        }
    }
}