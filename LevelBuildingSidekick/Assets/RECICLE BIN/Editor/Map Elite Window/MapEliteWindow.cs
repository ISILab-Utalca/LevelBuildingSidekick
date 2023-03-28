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
using LBS.Components;

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

        public int ButtonSize = 128; //Should be a RangeSlider field(!!!)

        //public GenericLBSWindow populationWindow;

        public ButtonWrapper[] Content;
        public VisualElement Container;
        private List<Vector2Int> toUpdate;

        public Vector2Field Partitions;
        public Label labelX;
        public Label labelY;

        private Texture2D ButtonBackground;
        public Texture2D defaultButton;

        public ClassDropDown OptimizeCategory;
        public ClassDropDown EvaluatorFieldX; //Deberian ser su propia clase con un Type para actualizar opciones(!)
        public ClassDropDown EvaluatorFieldY;

        public Button CalculateButton;

        private Color Paused;
        private Color Running;
        private object locker = new object();

        private MapElites mapElites;
        private LBSModule module;


        public void CreateGUI()
        {
            toUpdate = new List<Vector2Int>();
            VisualElement root = rootVisualElement;

            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteUXML");
            visualTree.CloneTree(root);

            var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            root.styleSheets.Add(styleSheet);

            mapElites = new MapElites();

            this.Container = root.Q<VisualElement>("Content");

            this.EvaluatorFieldX = root.Q<ClassDropDown>("EvaluatorFieldX");//new ClassDropDown(typeof(IRangedEvaluator), true);
            EvaluatorFieldX.Type = typeof(IRangedEvaluator);
            EvaluatorFieldX.FilterAbstract = true;

            this.EvaluatorFieldY = root.Q<ClassDropDown>("EvaluatorFieldY");//new ClassDropDown(typeof(IRangedEvaluator), true);
            EvaluatorFieldY.Type = typeof(IRangedEvaluator);
            EvaluatorFieldY.FilterAbstract = true;

            this.OptimizeCategory = root.Q<ClassDropDown>("BaseEvaluator");//new ClassDropDown(typeof(IEvaluator), true);
            OptimizeCategory.Type = typeof(IRangedEvaluator);
            OptimizeCategory.FilterAbstract = true;

            this.CalculateButton = root.Q<Button>("Calculate");
            this.Partitions = root.Q<Vector2Field>("Partitions");

            this.labelX = root.Q<Label>("LabelX");
            this.labelY = root.Q<Label>("LabelY");

            this.Partitions.RegisterValueChangedCallback(x => ChangePartitions(x.newValue));

            if(mapElites.XEvaluator!= null)
                EvaluatorFieldX.Value = mapElites.XEvaluator.ToString();
            EvaluatorFieldX.RegisterCallback<ChangeEvent<string>>(e => {
                labelX.text = (e!= null) ? e.newValue : "Evaluation X";
                var value = EvaluatorFieldX.GetChoiceInstance();
                mapElites.XEvaluator = value as IRangedEvaluator;
            });

            if (mapElites.YEvaluator != null)
                EvaluatorFieldY.value = mapElites.YEvaluator.ToString();
            EvaluatorFieldY.RegisterCallback<ChangeEvent<string>>(e => {
                labelY.text = (e != null) ? e.newValue : "Evaluation Y";
                var value = EvaluatorFieldY.GetChoiceInstance();
                mapElites.YEvaluator = value as IRangedEvaluator;
            });

            mapElites.OnSampleUpdated += UpdateSample;

            this.Partitions.value = new Vector2(3,3);

            CalculateButton.clicked += Run;

            Paused = root.style.backgroundColor.value;
            Running = Color.blue;
            
        }

        public void Run()
        {
            Clear();

            var adam = CreateAdam(module);

            if(adam == null)
            {
                throw new Exception("[ISI Lab] There is no suitable chromosome for class: " + module.GetType().Name );
            }

            mapElites.Adam = adam;

            mapElites.Run();
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

        private IChromosome CreateAdam(LBSModule module)
        {
            var type = module.GetType();

            var target = Reflection.GetClassesWith<ChromosomeFromModuleAttribute>().Where(t => t.Item2.Any(v => v.type == type)).First().Item1;

            var chrom = Activator.CreateInstance(target, new object[] { module }) as IChromosome;

            return chrom;
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