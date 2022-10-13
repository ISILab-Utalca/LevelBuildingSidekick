using LBS.ElementView;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using GeneticSharp.Domain;
using Commons.Optimization.Evaluator;
using Utility;
using Commons.Optimization;

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

        private MapElites mapElites;

        public int ButtonSize = 64; //Should be a RangeSlider field(!!!)

        public GenericGraphWindow mainView;

        public Button CalculateButton;

        public Vector2Field Partitions;

        public Texture2D defaultButton;
        public ButtonWrapper[] Content;
        public VisualElement Container;

        public ClassDropDown EvaluatorFieldX; //Deberian ser su propia clase con un Type para actualizar opciones(!)
        public ClassDropDown EvaluatorFieldY;

        public ClassDropDown IAField;

        public SubPanel evaluatorXPanel;
        public SubPanel evaluatorYPanel;
        public SubPanel optimizerPanel;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteUXML");
            visualTree.CloneTree(root);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            root.styleSheets.Add(styleSheet);

            mapElites = new MapElites();

            this.Container = root.Q<VisualElement>("Content");
            this.EvaluatorFieldX = new ClassDropDown(root.Q<DropdownField>("EvaluatorFieldX"), typeof(IRangedEvaluator), true);
            this.EvaluatorFieldY = new ClassDropDown(root.Q<DropdownField>("EvaluatorFieldY"), typeof(IRangedEvaluator), true);
            this.IAField = new ClassDropDown(root.Q<DropdownField>("IAField"), typeof(IOptimizer), true);
            this.CalculateButton = root.Q<Button>("Calculate");
            this.Partitions = root.Q<Vector2Field>("Partitions");

            this.evaluatorXPanel = root.Q<SubPanel>("EvaluatorX");
            evaluatorXPanel.style.display = DisplayStyle.None;
            this.evaluatorYPanel = root.Q<SubPanel>("EvaluatorY");
            evaluatorYPanel.style.display = DisplayStyle.None;
            this.optimizerPanel = root.Q<SubPanel>("Optimizer");
            optimizerPanel.style.display = DisplayStyle.None;

            this.Partitions.RegisterValueChangedCallback(x => ChangePartitions(x.newValue));

            EvaluatorFieldX.Dropdown.RegisterCallback<ChangeEvent<string>>(s => {
                evaluatorXPanel.style.display = DisplayStyle.Flex;
                var value = EvaluatorFieldX.GetChoiceInstance();
                mapElites.XEvaluator = value as IRangedEvaluator;
                if(value is IShowable)
                    evaluatorXPanel.SetValue(value as IShowable, "Evaluator: " + mapElites.XEvaluator.GetName(), "(axis X)");
            });

            EvaluatorFieldY.Dropdown.RegisterCallback<ChangeEvent<string>>(s => {
                evaluatorYPanel.style.display = DisplayStyle.Flex;
                var value = EvaluatorFieldY.GetChoiceInstance();
                mapElites.YEvaluator = value as IRangedEvaluator;
                if (value is IShowable)
                    evaluatorYPanel.SetValue(value as IShowable, "Evaluator: " + mapElites.YEvaluator.GetName(), "(axis Y)");
            });

            IAField.Dropdown.RegisterCallback<ChangeEvent<string>>(s => {
                optimizerPanel.style.display = DisplayStyle.Flex;
                var value = IAField.GetChoiceInstance();
                mapElites.Optimizer = value as IOptimizer;
                if (value is IShowable)
                    optimizerPanel.SetValue(value as IShowable,"Optimizer: " + mapElites.Optimizer.GetName());
            });

            mapElites.OnSampleUpdated += UpdateSample;

            this.Partitions.value = new Vector2(10,10);

            //this.fieldIA.RegisterValueChangedCallback(x => ChangeIA(x));

            CalculateButton.clicked += Run;
        }

        

        public void Run()
        {
            mapElites.Run();
        }

        private void OnFocus()
        {
            var il = Reflection.MakeGenericScriptable(mapElites);
            Selection.SetActiveObjectWithContext(il, il);
        }

        public void ChangePartitions(Vector2 partitions)
        {
            if (partitions.x == mapElites.XSampleCount && partitions.y == mapElites.YSampleCount)
                return;
            mapElites.XSampleCount = (int)partitions.x;
            mapElites.YSampleCount = (int)partitions.y;

            Content = new ButtonWrapper[mapElites.XSampleCount * mapElites.YSampleCount];
            Container.Clear();

            Container.style.width = 6 + (ButtonSize + 6) * mapElites.XSampleCount; // & es un padding que le asigna de forma automatica, no se de donde saca el valor

            for (int i = 0; i < Content.Length; i++)
            {
                var b = new ButtonWrapper(null, new Vector2(ButtonSize, ButtonSize));
                b.text = (i + 1).ToString();
                b.style.backgroundImage = defaultButton;
                Content[i] = b;
                Container.Add(b);
            }
        }

        public void UpdateSample(Vector2Int coords)
        {
            var index = (coords.y * mapElites.XSampleCount + coords.x);
            Content[index].Data = mapElites.BestSamples[coords.x, coords.y];
        }
    }
}