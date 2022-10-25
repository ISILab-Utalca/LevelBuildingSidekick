using LBS;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class TileEditWindow : EditorWindow, IHasCustomMenu
{
    public RenderObjectPivot pref;
    public RenderTexture rTexture;

    private Button addButton;
    private VisualElement content;
    private DropdownField[] dropdowns = new DropdownField[4];
    private Slider distance;
    private Slider rotation;
    private Slider labelDist;

    private Button generateMaoBtn;
    private Vector2IntField mapSize;
    private FloatField tileSize;

    private TileConections selected;
    private RenderObjectView screen;

    private List<Button> buttons = new List<Button>();

    private readonly int btnSize = 64;

    public Action<GameObject> SelectPref;

    public WFCSolver solverWFC = new WFCSolver(); // <--

    [MenuItem("ISILab/LBS plugin/Tile edit window", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<TileEditWindow>();
        window.titleContent = new GUIContent("Tile Edit Window");
    }

    void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
    {
        GUIContent content = new GUIContent("Clear prefabs refenreces");
        menu.AddItem(content, false, () =>
        {
            this.content.Clear();
            EditorPrefs.SetString("TileEditorWindow", "");
            this.content.Add(addButton);
            var tc = DirectoryTools.GetScriptables<TileConections>();
            var paths = tc.Select(so => AssetDatabase.GetAssetPath(so)).ToList();
            paths.ForEach(p => AssetDatabase.DeleteAsset(p));
        });
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileEditWindowUXML");
        visualTree.CloneTree(root);

        this.dropdowns[0] = root.Q<DropdownField>("A");
        this.dropdowns[1] = root.Q<DropdownField>("B");
        this.dropdowns[2] = root.Q<DropdownField>("C");
        this.dropdowns[3] = root.Q<DropdownField>("D");
        ShowDropdown(false);
        

        this.screen = root.Q<RenderObjectView>();
        this.content = root.Q<VisualElement>("Content");
        
        this.addButton = root.Q<Button>("AddButton");
        addButton.clicked += AddAction;
        this.distance = root.Q<Slider>("Distance");
        this.rotation = root.Q<Slider>("Rotation");
        this.labelDist = root.Q<Slider>("LabelDistance");

        this.generateMaoBtn = root.Q<Button>("GenerateButton");
        generateMaoBtn.clicked += () => GenerateMap(SolveMap());
        this.mapSize = root.Q<Vector2IntField>("mapSize");

        ActualizeView();

        var pivot = CreatePivot();
        distance.RegisterValueChangedCallback((e)=> { pivot.SetDistanceCam(e.newValue); });
        pivot.SetDistanceCam(distance.value);
        rotation.RegisterValueChangedCallback((e) => { pivot.SetRotateCam(e.newValue); });
        pivot.SetRotateCam(rotation.value);
        labelDist.RegisterValueChangedCallback((e) => { pivot.LabelDist(e.newValue); });
        pivot.LabelDist(labelDist.value);
        SelectPref += pivot.SetPref;

    }

    public TileConections[,] SolveMap()
    {
        var samples = DirectoryTools.GetScriptables<TileConections>();
        TileConections[,] tileColection = new TileConections[mapSize.value.x, mapSize.value.y];
        return solverWFC.Solve(samples,tileColection);
    }

    public void GenerateMap(TileConections[,] map)
    {
        var pivot = new GameObject();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                // falta ponerle la rotacion correcta (!!!!!)
                var t = SceneView.Instantiate(map[i, j].Tile, pivot.transform);
                t.transform.position *= tileSize.value;
            }
        }
    }

    public void ShowDropdown(bool v)
    {
        foreach (var dd in dropdowns)
        {
            dd.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }


    public RenderObjectPivot CreatePivot()
    {
        var pivot = SceneView.Instantiate(pref);
        var muyLejos = 9999;
        pivot.transform.position = new Vector3(muyLejos, muyLejos, muyLejos);
        pivot.hideFlags = HideFlags.HideAndDontSave;
        return pivot;
    }

    private void AddAction()
    {
        var path = EditorUtility.OpenFilePanel("Load prefab", "", "prefab");
        path = DirectoryTools.FullPathToProjectPath(path);

        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var so = ScriptableObject.CreateInstance<TileConections>();
        so.Init(go);

        var sPath = EditorUtility.SaveFilePanel("Save metadata","", "","asset");
        sPath = DirectoryTools.FullPathToProjectPath(sPath);
        AssetDatabase.CreateAsset(so, sPath);
        EditorUtility.SetDirty(so);
        ActualizeView();
    }

    private void SetSelected(TileConections data)
    {
        this.selected = data;
        SelectPref?.Invoke(selected.Tile);
        var i = 0;
        ShowDropdown(true);
        foreach (var dd in dropdowns)
        {
            dd.value = data.GetConnection(i);
            dd.choices = LBSTags.GetInstance("WFC Tags").Alls;
            int n = i;
            dd.RegisterCallback<ChangeEvent<string>>(e => {
                this.selected.SetConnection(n, e.newValue);
                EditorUtility.SetDirty(data);
            });
            i++;
        }
    }

    private void LoadButtons()
    {
        var tiles = DirectoryTools.GetScriptables<TileConections>();
        foreach (var data in tiles)
        {
            var img = AssetPreview.GetAssetPreview(data.Tile);
            var btn = new Button();
            btn.clicked += () => {
                var selected = data;
                SetSelected(selected);
            };
            btn.style.width = btn.style.height = btn.style.maxHeight = btn.style.minHeight = btnSize;
            btn.style.backgroundImage = img;
            buttons.Add(btn);
            content.Add(btn);
        }
    }

    private void ActualizeView()
    {
        content.Clear();
        LoadButtons();
        content.Add(addButton);
    }

}