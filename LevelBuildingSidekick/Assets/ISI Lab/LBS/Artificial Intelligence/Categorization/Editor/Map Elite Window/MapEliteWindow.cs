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
using LBS.Components.TileMap;

public class MapEliteWindow : EditorWindow
{
    [MenuItem("ISILab/LBS plugin/MapEliteWindow", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<MapEliteWindow>();
        window.titleContent = new GUIContent("Map Elite");
        window.Clear();
    }


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

    private Texture2D ButtonBackground;
    public Texture2D defaultButton;

    public ClassFoldout FitnessField;
    public ClassFoldout EvaluatorFieldX; //Deberian ser su propia clase con un Type para actualizar opciones(!)
    public ClassFoldout EvaluatorFieldY;


    private Color Paused;
    private Color Running;
    private object locker = new object();

    public MapElites mapElites;

    LBSLayer layer;


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

        this.Partitions = root.Q<Vector2Field>("Partitions");
        this.ModuleField = root.Q<DropdownField>(name: "ModuleField");
        this.BackgroundField = root.Q<DropdownField>(name: "BackgroundField");
        this.OptimizerField = root.Q<ClassDropDown>(name: "OptimizerField");
        this.CalculateButton = root.Q<Button>("Calculate");

        this.labelX = root.Q<Label>("LabelX");
        this.labelY = root.Q<Label>("LabelY");

        this.Partitions.RegisterValueChangedCallback(x => ChangePartitions(x.newValue));

        if (layer != null)
            SetLayer(layer);

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
            var ve = (FitnessField.content as EvaluatorVE);
            var value = ve.Evaluator;
            mapElites.Optimizer.Evaluator = value as IRangedEvaluator;
            ve.SetLayer(layer);
        });



        mapElites.OnSampleUpdated += UpdateSample;

        this.Partitions.value = new Vector2(3,3);

        CalculateButton.clicked += Run;
        toUpdate.Clear();

        Paused = root.style.backgroundColor.value;
        Running = Color.blue;
            
    }

    public void Run()
    {
        Clear();
        toUpdate.Clear();

        var module = layer.GetModule<LBSModule>(ModuleField.value);

        (EvaluatorFieldX.content as EvaluatorVE).Init();
        (EvaluatorFieldY.content as EvaluatorVE).Init();
        (FitnessField.content as EvaluatorVE).Init();

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
        Debug.Log(ButtonBackground.width + " - " + ButtonBackground.height);
        foreach (ButtonWrapper bw in Content)
        {
            bw.style.backgroundImage = ButtonBackground;
            Debug.Log(bw.style.width + " - " + bw.style.height);
            bw.Data = null;
        }
    }

    private Texture2D BackgroundTexture(LBSModule mod)
    {
        var target = Reflection.GetClassesWith<ModuleTexturizerAttribute>().Where(t => t.Item2.Any(v => v.type == mod.GetType())).First().Item1;
        var texturizer = Activator.CreateInstance(target) as ModuleTexturizer;
        var t = texturizer.ToTexture(mod);
        
        return t;
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
                Content[index].SetTexture(ButtonBackground.MergeTextures(t));
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


    public void SetLayer(LBSLayer layer)
    {
        this.layer = layer;
        BackgroundField.choices = new List<string>();

        var mods = layer.Parent.Layers.SelectMany(l => l.Modules);

        foreach (var m in mods)
        {
            if (Reflection.GetClassesWith<ModuleTexturizerAttribute>().Any(t => t.Item2.Any(v => v.type == m.GetType())))
            {
                BackgroundField.choices.Add(m.Key);
            }
        }

        if(BackgroundField.choices.Count != 0)
        {
            BackgroundField.value = BackgroundField.choices[0];
            BackgroundField.RegisterValueChangedCallback(e => ButtonBackground = BackgroundTexture(mods.ToList().Find(m => m.Key == e.newValue)));
            var m = mods.ToList().Find(m => m.Key == BackgroundField.value);
            Debug.Log((m as LBSSchema).GetRect());
            ButtonBackground = BackgroundTexture(m);
        }

        ModuleField.choices = new List<string>();

        foreach (var m in layer.Modules)
        {
            if (Reflection.GetClassesWith<ChromosomeFromModuleAttribute>().Any(t => t.Item2.Any(v => v.type == m.GetType())))
            {
                ModuleField.choices.Add(m.Key);
            }
        }

        if (ModuleField.choices.Count != 0)
        {
            ModuleField.value = ModuleField.choices[0];
        }
    }
}
