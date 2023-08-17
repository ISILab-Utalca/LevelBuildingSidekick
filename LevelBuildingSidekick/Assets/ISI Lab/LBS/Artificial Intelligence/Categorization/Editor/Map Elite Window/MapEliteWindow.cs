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
using ISILab.AI.Optimization;
using GeneticSharp.Domain.Chromosomes;
using System;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Components.Teselation;

public class MapEliteWindow : EditorWindow
{
    [MenuItem("ISILab/LBS plugin/MapEliteWindow", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<MapEliteWindow>();
        window.titleContent = new GUIContent("Map Elite");
        window.Clear();
    }

    public Texture2D img, img2, img3;


    public int ButtonSize = 128; //Should be a RangeSlider field(!!!)

    //public GenericLBSWindow populationWindow;

    public ButtonWrapper[] Content;
    public VisualElement Container;
    private List<Vector2Int> toUpdate;

    public Label labelX;
    public Label labelY;

    public Vector2Field Partitions;
    public DropdownField ModuleField;
    public DropdownField BackgroundField;
    public ClassDropDown OptimizerField;
    public Button CalculateButton;
    private Button ContinueButton;
    private Texture2D ButtonBackground;
    public Texture2D defaultButton;

    public ClassFoldout FitnessField;
    public ClassFoldout EvaluatorFieldX;
    public ClassFoldout EvaluatorFieldY;

    public MinMaxSlider XThreshold;
    public MinMaxSlider YThreshold;


    private Color Paused;
    private Color Running;
    private object locker = new object();

    public MapElites mapElites;

    public TileMapModule backgroundModule;

    LBSLayer layer;


    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var assitant = new AssistantMapElite();
        var ve = new AssistantMapEliteVE(assitant);
        root.Add(ve);


        /*img = DirectoryTools.SearchAssetByName<Texture2D>("PausedProcess");
        img2 = DirectoryTools.SearchAssetByName<Texture2D>("LoadingContent");
        img3 = DirectoryTools.SearchAssetByName<Texture2D>("ContentNotFound");

        toUpdate = new List<Vector2Int>();
        VisualElement root = rootVisualElement;

        var visualTree = LBSAssetsStorage.Instance.Get<VisualTreeAsset>().Find(o => o.name == "MapEliteUXML");
        visualTree.CloneTree(root);

        var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
        var s2 = EditorGUIUtility.Load("DefaultCommonDark.uss") as StyleSheet;
        root.styleSheets.Add(s2);
        root.styleSheets.Add(styleSheet);

        mapElites = new MapElites();


        this.Container = root.Q<VisualElement>("Content");

        this.Partitions = root.Q<Vector2Field>("Partitions");
        this.ModuleField = root.Q<DropdownField>(name: "ModuleField");
        this.BackgroundField = root.Q<DropdownField>(name: "BackgroundField");
        BackgroundField.RegisterValueChangedCallback(e =>
        {
            var mods = layer.Parent.Layers.SelectMany(l => l.Modules);
            backgroundModule = mods.ToList().Find(m => m.ID == e.newValue) as TileMapModule;
            Clear();
        });

        if(layer != null)
            SetLayer(layer);

        this.OptimizerField = root.Q<ClassDropDown>(name: "OptimizerField");
        this.CalculateButton = root.Q<Button>("Calculate");
        this.ContinueButton = root.Q<Button>("Continue");

        this.labelX = root.Q<Label>("LabelX");
        this.labelY = root.Q<Label>("LabelY");

        this.Partitions.RegisterValueChangedCallback(x => ChangePartitions(x.newValue));

        this.XThreshold = root.Q<MinMaxSlider>("XThreshold");
        XThreshold.minValue = mapElites.XThreshold.x;
        XThreshold.maxValue = mapElites.XThreshold.y;
        XThreshold.RegisterValueChangedCallback((evt) => mapElites.XThreshold = evt.newValue);


        this.YThreshold = root.Q<MinMaxSlider>("YThreshold");
        YThreshold.minValue = mapElites.YThreshold.x;
        YThreshold.maxValue = mapElites.YThreshold.y;
        YThreshold.RegisterValueChangedCallback((evt) => mapElites.YThreshold = evt.newValue);


        OptimizerField.Type = typeof(BaseOptimizer);
        OptimizerField.value = OptimizerField.choices[0];
        mapElites.Optimizer = OptimizerField.GetChoiceInstance() as BaseOptimizer;
        //Debug.Log(OptimizerField.value);
        OptimizerField.RegisterValueChangedCallback(
            (e) =>
            {
                mapElites.Optimizer = OptimizerField.GetChoiceInstance() as BaseOptimizer;
            });


        this.EvaluatorFieldX = root.Q<ClassFoldout>("EvaluatorFieldX");//new ClassDropDown(typeof(IRangedEvaluator), true);
        EvaluatorFieldX.dropdown.Type = typeof(IRangedEvaluator);
        EvaluatorFieldX.dropdown.FilterAbstract = true;

        this.EvaluatorFieldY = root.Q<ClassFoldout>("EvaluatorFieldY");//new ClassDropDown(typeof(IRangedEvaluator), true);
        EvaluatorFieldY.dropdown.Type = typeof(IRangedEvaluator);
        EvaluatorFieldY.dropdown.FilterAbstract = true;

        if (mapElites.XEvaluator!= null)
            EvaluatorFieldX.dropdown.Value = mapElites.XEvaluator.ToString();
        EvaluatorFieldX.dropdown.RegisterValueChangedCallback(e => {
            labelX.text = (e.newValue != null) ? e.newValue : "Evaluation X";
            var ve = (EvaluatorFieldX.content as EvaluatorVE);
            var value = ve.Evaluator;
            mapElites.XEvaluator = value as IRangedEvaluator;
            ve.SetLayer(layer);
        });

        /*
        if (mapElites.YEvaluator != null)
            EvaluatorFieldY.dropdown.value = mapElites.YEvaluator.ToString();
        EvaluatorFieldY.dropdown.RegisterValueChangedCallback(e => {
            labelY.text = (e != null) ? e.newValue : "Evaluation Y";
            var ve = (EvaluatorFieldY.content as EvaluatorVE);
            var value = ve.Evaluator;
            mapElites.YEvaluator = value as IRangedEvaluator;
            ve.SetLayer(layer);
        });

        this.FitnessField = root.Q<ClassFoldout>("FitnessField");//new ClassDropDown(typeof(IEvaluator), true);
        FitnessField.dropdown.Type = typeof(IRangedEvaluator);
        FitnessField.dropdown.FilterAbstract = true;

        if (mapElites.Optimizer != null && mapElites.Optimizer.Evaluator != null)
            FitnessField.dropdown.value = mapElites.Optimizer.Evaluator.ToString();
        FitnessField.dropdown.RegisterValueChangedCallback(e => {
            labelY.text = (e != null) ? e.newValue : "Fitness";
            var ve = (FitnessField.content as EvaluatorVE);
            var value = ve.Evaluator;
            mapElites.Optimizer.Evaluator = value as IRangedEvaluator;
            mapElites.YEvaluator = value as IRangedEvaluator;
            ve.SetLayer(layer);
        });



        mapElites.OnSampleUpdated += UpdateSample;
        

        this.Partitions.value = new Vector2(3,3);
        ButtonBackground = defaultButton;
        ChangePartitions(Partitions.value);

        CalculateButton.clicked += Run;
        ContinueButton.clicked += Continue;
        toUpdate.Clear();

        Paused = root.style.backgroundColor.value;
        Running = new Color(0.22f,0.22f,0.36f);//Color.blue;*/
    }

    #region Done

    public void Run()
    {
        Clear();
        toUpdate.Clear();

        // set all wrapper to loading icon
        foreach (ButtonWrapper bw in Content)
        {
            bw.style.backgroundImage = img3;
            bw.Data = null;
            bw.Text = "";
            bw.UpdateLabel();
        }


        var module = layer.GetModule<LBSModule>(ModuleField.value);

        (EvaluatorFieldX.content as EvaluatorVE).Init();
        //(EvaluatorFieldY.content as EvaluatorVE).Init();
        (FitnessField.content as EvaluatorVE).Init();

        var adam = CreateAdam(module);

        if (adam == null)
        {
            throw new Exception("[ISI Lab] There is no suitable chromosome for class: " + module.GetType().Name);
        }

        mapElites.Adam = adam;

        mapElites.Run();
    }
    public void ChangePartitions(Vector2 partitions)
    {
        //ButtonBackground = BackgroundTexture(layer.GetModule<LBSModule>(BackgroundField.value));
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
            b.clicked += () =>
            {
                if (b.Data != null)
                {
                    var mod = layer.GetModule<LBSModule>(ModuleField.value);
                    var chrom = b.Data as LBSChromosome;

                    if (mod == null)
                    {
                        throw new Exception("[ISI Lab] Module " + ModuleField.value + " could not be found!");
                    }

                    if (chrom == null)
                    {
                        throw new Exception("[ISI Lab] Data " + b.Data.GetType().Name + " is not LBSChromosome!");
                    }

                    //mod.Rewrite(chrom.ToModule());

                    GetWindow<LBSMainWindow>().Repaint();

                }
            };
            Content[i] = b;
            Container.Add(b);
        }

        Content.ToList().ForEach(b => b.style.backgroundImage = img);
    }
    public void UpdateSample(Vector2Int coords)
    {
        var index = (coords.y * mapElites.XSampleCount + coords.x);
        if (Content[index].Data != null && (Content[index].Data as IOptimizable).Fitness > mapElites.BestSamples[coords.y, coords.x].Fitness)
        {
            return;
        }
        Content[index].Data = mapElites.BestSamples[coords.y, coords.x];
        Content[index].Text = ((decimal)mapElites.BestSamples[coords.y, coords.x].Fitness).ToString("f4");



        lock (locker)
        {
            if (!toUpdate.Contains(coords))
                toUpdate.Add(coords);
        }
    }
    private void OnInspectorUpdate()
    {/*
        lock (locker)
        {
            for (int i = 0; i < toUpdate.Count; i++)
            {
                var v = toUpdate[i];
                var index = (v.y * mapElites.XSampleCount + v.x);
                var t = Content[index].GetTexture();
                if (Content[index].Data != null)
                {
                    ButtonBackground = BackgroundTexture(backgroundModule);
                    Content[index].SetTexture(ButtonBackground.MergeTextures(t));
                }
                else
                {
                    Content[index].SetTexture(img3);
                }
                Content[index].UpdateLabel();
            }
            toUpdate.Clear();
        }

        if (mapElites.Finished)
        {
            foreach (ButtonWrapper bw in Content)
            {
                if (bw.Data == null)
                {
                    bw.style.backgroundImage = img2;
                }
            }
        }

        */
    }

    #endregion

    public void Continue()
    {
        if(mapElites.Optimizer.State == Op_State.TerminationReached)
        {
            (EvaluatorFieldX.content as EvaluatorVE).Init();
            (EvaluatorFieldY.content as EvaluatorVE).Init();
            (FitnessField.content as EvaluatorVE).Init();

            mapElites.Restart();
        }
    }

    public void Clear()
    {/*
        foreach (var button in Content)
        {
            button.style.backgroundImage = img;
        }

        /*
        ButtonBackground = BackgroundTexture(backgroundModule);
        foreach (ButtonWrapper bw in Content)
        {
            bw.style.backgroundImage = ButtonBackground;
            bw.Data = null;
            bw.Text = "";
            bw.UpdateLabel();
        }
        */
    }

    private Texture2D BackgroundTexture(LBSModule mod)
    {
        var target = Reflection.GetClassesWith<ModuleTexturizerAttribute>().Where(t => t.Item2.Any(v => v.type == mod.GetType())).First().Item1;
        var texturizer = Activator.CreateInstance(target) as ModuleTexturizer;
        var t = texturizer.ToTexture(mod);
        
        return t;
    }

    private ChromosomeBase CreateAdam(LBSModule module)
    {
        var type = module.GetType();

        /*var target = Reflection.GetClassesWith<ChromosomeFromBehaviourAttribute>().Where(t => t.Item2.Any(v => v.type == type)).First().Item1;

        //var immutables = backgroundModule.EmptyIndexes().ToArray();

        var chrom = Activator.CreateInstance(target, new object[] { module, backgroundModule.GetBounds()}) as ChromosomeBase;

        return chrom;*/
        return null;

    }



    public void SetLayer(LBSLayer layer)
    {
        this.layer = layer;
        BackgroundField.choices = GetTexturizables();

        if(BackgroundField.choices.Count != 0)
        {
            BackgroundField.value = BackgroundField.choices[0];
            backgroundModule = this.layer.Parent.Layers.SelectMany(m => m.Modules).ToList().Find(m => m.ID == BackgroundField.value) as TileMapModule;

        }

        Clear();

        ModuleField.choices = GetChromosomables();

        if (ModuleField.choices.Count != 0)
        {
            ModuleField.value = ModuleField.choices[0];
        }
    }

    private List<string> GetTexturizables()
    {
        List<string> choices = new List<string>();

        var mods = layer.Parent.Layers.SelectMany(l => l.Modules);

        foreach (var m in mods)
        {
            if (m is TileMapModule && Reflection.GetClassesWith<ModuleTexturizerAttribute>().Any(t => t.Item2.Any(v => v.type == m.GetType())))
            {
                choices.Add(m.ID);
            }
        }

        return choices;
    }

    private List<string> GetChromosomables()
    {
        List<string> choices = new List<string>();

        foreach (var m in layer.Modules)
        {
            /*var ves = Utility.Reflection.GetClassesWith<ChromosomeFromBehaviourAttribute>().Where(t => t.Item2.Any(v => v.type == m.GetType()));
            if (ves.Count() != 0)
            {
                choices.Add(m.ID);
            }*/
        }

        return choices;
    }
}
