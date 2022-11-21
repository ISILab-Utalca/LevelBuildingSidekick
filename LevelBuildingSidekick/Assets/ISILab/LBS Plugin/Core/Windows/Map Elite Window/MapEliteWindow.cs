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
using GeneticSharp.Domain.Chromosomes;
using LBS.Representation.TileMap;

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

            EvaluatorFieldX.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                evaluatorXPanel.style.display = DisplayStyle.Flex;
                var value = EvaluatorFieldX.GetChoiceInstance();
                mapElites.XEvaluator = value as IRangedEvaluator;
                if(value is IShowable)
                    evaluatorXPanel.SetValue(value as IShowable, "Evaluator: " + mapElites.XEvaluator.GetName(), "(axis X)");
            });

            EvaluatorFieldY.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                evaluatorYPanel.style.display = DisplayStyle.Flex;
                var value = EvaluatorFieldY.GetChoiceInstance();
                mapElites.YEvaluator = value as IRangedEvaluator;
                if (value is IShowable)
                    evaluatorYPanel.SetValue(value as IShowable, "Evaluator: " + mapElites.YEvaluator.GetName(), "(axis Y)");
            });

            IAField.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
                optimizerPanel.style.display = DisplayStyle.Flex;
                var value = IAField.GetChoiceInstance();
                mapElites.Optimizer = value as IOptimizer;
                if (value is IShowable)
                    optimizerPanel.SetValue(value as IShowable,"Optimizer: " + mapElites.Optimizer.GetName());
            });

            mapElites.OnSampleUpdated += UpdateSample;

            this.Partitions.value = new Vector2(3,3);

            //ChangePartitions(new Vector2(3, 3));

            //this.fieldIA.RegisterValueChangedCallback(x => ChangeIA(x));

            CalculateButton.clicked += Run;
        }

        public void Run()
        {
            Clear();
            if(!(mainView.CurrentController is IChromosomable))
            {
                return;
            }

            StampTileMapChromosome.TileMap = mainView.GetController<LBSTileMapController>();
            mapElites.Adam = (mainView.CurrentController as IChromosomable).ToChromosome();
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
                b.clicked += () => 
                { 
                    if (b.Data != null)
                    {
                        (mainView.CurrentController as IChromosomable).FromChromosome(b.Data as IChromosome);
                    } 
                };
                Content[i] = b;
                Container.Add(b);
            }

            var t = BackgroundTexture();

            Content.ToList().ForEach(b => b.style.backgroundImage = t);
        }

        public void UpdateSample(Vector2Int coords)
        {
            var index = (coords.y * mapElites.XSampleCount + coords.x);
            Content[index].Data = mapElites.BestSamples[coords.x, coords.y];
        }

        public void Clear()
        {
            foreach(ButtonWrapper bw in Content)
            {
                bw.style.backgroundImage = defaultButton;
                bw.Data = null;
            }
        }

        private Texture2D BackgroundTexture()
        {
            var tmc = (mainView.GetController<LBSTileMapController>());
            var rooms = (tmc.GetData() as LBSSchemaData).GetRooms();
            var tiles = rooms.SelectMany(r => r.Tiles);

            var x1 = tiles.Min(t => t.GetPosition().x);
            var x2 = tiles.Max(t => t.GetPosition().x);

            var y1 = tiles.Min(t => t.GetPosition().y);
            var y2 = tiles.Max(t => t.GetPosition().y);

            int width = (x2 - x1) + 1;
            int height = (y2 - y1) + 1;

            var size = new Vector2Int(width, height);
            var offset = new Vector2(x1, y1);

            var pref = Resources.Load<Texture2D>("Floor");
            var texture = new Texture2D((int)(width*tmc.TileSize), (int)(height*tmc.TileSize));

            foreach(var r in rooms)
            {
                var aux = new Texture2D(pref.width, pref.height);
                var pixels = pref.GetPixels();
                var color = r.Color;
                for(int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color(pixels[i].r * color.r, pixels[i].g * color.g, pixels[i].b * color.b);
                }
                aux.SetPixels(pixels);
                aux.Apply();
                foreach(var t in r.Tiles)
                {
                    var pos = t.GetPosition();
                    texture.InsertTextureInRect(aux, (int)(pos.x * tmc.TileSize), (int)((height - 1 - pos.y) * tmc.TileSize), (int)tmc.TileSize, (int)tmc.TileSize);
                }
            }
            texture.Apply();

            return texture;
        }
    }
}