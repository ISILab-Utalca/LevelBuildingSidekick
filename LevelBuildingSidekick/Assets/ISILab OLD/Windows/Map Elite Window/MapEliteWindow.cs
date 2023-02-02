using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using GeneticSharp.Domain;
using Commons.Optimization.Evaluator;
using Utility;
using Commons.Optimization;
using GeneticSharp.Domain.Chromosomes;
using System;

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

        public int ButtonSize = 128; //Should be a RangeSlider field(!!!)

        public GenericLBSWindow populationWindow;

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

        private List<Vector2Int> toUpdate;
        private Texture2D ButtonBackground;

        private Color Paused;
        private Color Running;
        private object locker = new object();

        public Label labelX;
        public Label labelY;

        public void CreateGUI()
        {/*
            populationWindow = EditorWindow.GetWindow<LBSPopulationWindow>();
            toUpdate = new List<Vector2Int>();
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

            this.labelX = root.Q<Label>("LabelX");
            this.labelY = root.Q<Label>("LabelY");

            this.evaluatorXPanel = root.Q<SubPanel>("EvaluatorX");
            evaluatorXPanel.style.display = mapElites.XEvaluator == null ? DisplayStyle.None : DisplayStyle.Flex;
            this.evaluatorYPanel = root.Q<SubPanel>("EvaluatorY");
            evaluatorYPanel.style.display = mapElites.YEvaluator == null ? DisplayStyle.None : DisplayStyle.Flex;
            this.optimizerPanel = root.Q<SubPanel>("Optimizer");
            optimizerPanel.style.display = mapElites.Optimizer == null ? DisplayStyle.None : DisplayStyle.Flex;

            this.Partitions.RegisterValueChangedCallback(x => ChangePartitions(x.newValue));

            if(mapElites.XEvaluator!= null)
                EvaluatorFieldX.Dropdown.value = mapElites.XEvaluator.ToString();
            EvaluatorFieldX.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                labelX.text = (e!= null) ? e.newValue : "Evaluation X";
                evaluatorXPanel.style.display = DisplayStyle.Flex;
                var value = EvaluatorFieldX.GetChoiceInstance();
                mapElites.XEvaluator = value as IRangedEvaluator;
                if(value is IShowable)
                    evaluatorXPanel.SetValue(value as IShowable, "Evaluator: " + mapElites.XEvaluator.GetName(), "(axis X)");
            });

            if (mapElites.YEvaluator != null)
                EvaluatorFieldY.Dropdown.value = mapElites.YEvaluator.ToString();
            EvaluatorFieldY.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                labelY.text = (e != null) ? e.newValue : "Evaluation Y";
                evaluatorYPanel.style.display = DisplayStyle.Flex;
                var value = EvaluatorFieldY.GetChoiceInstance();
                mapElites.YEvaluator = value as IRangedEvaluator;
                if (value is IShowable)
                    evaluatorYPanel.SetValue(value as IShowable, "Evaluator: " + mapElites.YEvaluator.GetName(), "(axis Y)");
            });

            if (mapElites.Optimizer != null)
                IAField.Dropdown.value = mapElites.Optimizer.ToString();
            IAField.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                optimizerPanel.style.display = DisplayStyle.Flex;
                var value = IAField.GetChoiceInstance();
                mapElites.Optimizer = value as IOptimizer;
                if (value is IShowable)
                    optimizerPanel.SetValue(value as IShowable,"Optimizer: " + mapElites.Optimizer.GetName());
            });

            mapElites.OnSampleUpdated += UpdateSample;

            this.Partitions.value = new Vector2(3,3);

            CalculateButton.clicked += Run;

            Paused = root.style.backgroundColor.value;
            Running = Color.blue;
            */
        }

        public void Run()
        {
            /*
            Clear();
            if(!(populationWindow.CurrentController is IChromosomable))
            {
                return;
            }

            StampTileMapChromosome.TileMap = populationWindow.GetController<LBSTileMapController>();
            mapElites.Adam = (populationWindow.CurrentController as IChromosomable).ToChromosome();
            mapElites.Run();
            */
        }

        private void OnFocus()
        {
            var il = Reflection.MakeGenericScriptable(mapElites);
            Selection.SetActiveObjectWithContext(il, il);
        }

        public void ChangePartitions(Vector2 partitions)
        {
            ButtonBackground = BackgroundTexture();
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
                b.clicked += () => 
                { 
                    if (b.Data != null)
                    {
                        //(populationWindow.CurrentController as IChromosomable).FromChromosome(b.Data as IChromosome);
                        //populationWindow.Repaint();
                    } 
                };
                Content[i] = b;
                Container.Add(b);
            }

            Content.ToList().ForEach(b => b.style.backgroundImage = ButtonBackground);
        }

        public void UpdateSample(Vector2Int coords)
        {
            var index = (coords.y * mapElites.XSampleCount + coords.x);
            Content[index].Data = mapElites.BestSamples[coords.x, coords.y];
            Content[index].Text =  ((decimal)mapElites.BestSamples[coords.x, coords.y].Fitness).ToString("f4");
            lock (locker)
            {
                if (!toUpdate.Contains(coords))
                    toUpdate.Add(coords);
            }
        }

        public void Clear()
        {
            foreach(ButtonWrapper bw in Content)
            {
                bw.style.backgroundImage = ButtonBackground;
                bw.Data = null;
            }
        }

        private Texture2D BackgroundTexture()
        {
            /*
            int tsize = 16;
            var tmc = (populationWindow.GetController<LBSTileMapController>());
            var rooms = (tmc.GetData() as LBSSchemaData).GetRooms();
            var tiles = rooms.SelectMany(r => r.TilesPositions);

            var x1 = tiles.Min(t => t.x);
            var x2 = tiles.Max(t => t.x);

            var y1 = tiles.Min(t => t.y);
            var y2 = tiles.Max(t => t.y);

            int width = (x2 - x1) + 1;
            int height = (y2 - y1) + 1;

            var size = width > height ? width : height;
            var offset = new Vector2Int(x1, y1);

            var pref = Resources.Load<Texture2D>("Floor");
            var texture = new Texture2D((size * tsize), (size * tsize));
            var pixels = texture.GetPixels();
            for(int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(0, 0, 0, 0);
            }
            texture.SetPixels(0, 0, (size * tsize), (size * tsize), pixels);
            texture.Apply();

            foreach (var r in rooms)
            {
                var aux = new Texture2D(pref.width, pref.height);
                pixels = pref.GetPixels();
                var color = r.Color;
                for(int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color(pixels[i].r * color.r, pixels[i].g * color.g, pixels[i].b * color.b);
                }
                aux.SetPixels(pixels);
                aux.Apply();
                foreach(var tp in r.TilesPositions)
                {
                    var pos = tp;
                    texture.InsertTextureInRect(aux, ((pos.x - offset.x) * tsize), ((height - 1 - (pos.y - offset.y)) * tsize), tsize, tsize);
                }
            }
            texture.Apply();

            return texture;
            */
            return null;
        }

        private void OnInspectorUpdate()
        {
            lock(locker)
            {
                for (int i = 0; i < toUpdate.Count; i++)
                {
                    var v = toUpdate[i]; 
                    var index = (v.y * mapElites.XSampleCount + v.x);
                    var t = Content[index].GetTexture();
                    Content[index].SetTexture(ButtonBackground.Merge(t));
                    Content[index].UpdateLabel();
                }
                toUpdate.Clear();
            }
            

            if (mapElites.Running)
            {
                rootVisualElement.style.backgroundColor = Running;
            }
            else
            {
                rootVisualElement.style.backgroundColor = Paused;
            }
        }
    }
}