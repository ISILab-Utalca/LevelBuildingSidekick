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
    private Slider weight;

    private Slider distance;
    private Slider rotation;
    private Slider labelDist;

    private Button generateMapBtn;
    private Vector2IntField mapSize;
    private FloatField tileSize;

    private Button generateSchema;

    public static TileConections selected;
    private RenderObjectView screen;

    private List<Button> buttons = new List<Button>();

    private readonly int btnSize = 64;

    public Action<GameObject> SelectPref;

    public WFCSolver solverWFC = new WFCSolver(); // <--

    private Color BASE;

   // [MenuItem("ISILab/LBS plugin/Tile edit window", priority = 1)]
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

        this.dropdowns[3] = root.Q<DropdownField>("Top");
        this.dropdowns[0] = root.Q<DropdownField>("Right");
        this.dropdowns[1] = root.Q<DropdownField>("Bottom");
        this.dropdowns[2] = root.Q<DropdownField>("Left");
        this.weight = root.Q<Slider>("Weight");
        ShowDropdown(false);
        

        this.screen = root.Q<RenderObjectView>();
        this.content = root.Q<VisualElement>("Content");
        
        this.addButton = root.Q<Button>("AddButton");
        addButton.clicked += AddAction;
        this.distance = root.Q<Slider>("Distance");
        this.rotation = root.Q<Slider>("Rotation");
        this.labelDist = root.Q<Slider>("LabelDistance");

        this.generateMapBtn = root.Q<Button>("GenerateButton");
        generateMapBtn.clicked += () => GenerateMap(SolveMap());
        this.generateSchema = root.Q<Button>("GenerateSchema");
        generateSchema.clicked += () =>
        {
            var s = SolveMap();
            var m = GridToMapData.Generate(s);
            LBSController.CurrentLevel.data.AddRepresentation(m);
        };
        this.mapSize = root.Q<Vector2IntField>("mapSize");
        this.tileSize = root.Q<FloatField>("tileSize");

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

    public TileConnectWFC[,] SolveMap()
    {
        var samples = DirectoryTools.GetScriptables<TileConections>();
        var x = new TileConnectWFC[mapSize.value.x, mapSize.value.y];
        return solverWFC.Solve(samples,x);
    }

    public void GenerateMap(TileConnectWFC[,] map)
    {
        var pivot = new GameObject();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if(map[i,j] == null)
                {
                    continue;
                }

                var pref = map[i, j].Tile.Tile;
                var go = SceneView.Instantiate(pref, pivot.transform);
                go.transform.position = new Vector3(i,0,-j) * tileSize.value;
                
                var v = map[i, j].Rotation;
                if (v % 2 != 0) // parche, no quitar (!!!)
                    v += 2;

                for (int k = 0; k < v; k++)
                    go.transform.Rotate(new Vector3(0, 1, 0),90);
            }
        }
    }

    public void Print(TileConnectWFC[,] map)
    {
        string v = "";
        for (int i = 0; i < map.GetLength(1); i++)
        {
            for (int k = 0; k < 3; k++)
            {
                for (int j = 0; j < map.GetLength(0); j++)
                {
                    v += map[j, i].Print(k);
                    v += "   ";
                }
                v += "\n";
            }
            v += "\n";
        }
        Debug.Log(v);
    }


    public void ShowDropdown(bool v)
    {
        foreach (var dd in dropdowns)
        {
            dd.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;
        }
        weight.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;
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
        TileEditWindow.selected = data;
        SelectPref?.Invoke(selected.Tile);
        var i = 0;
        ShowDropdown(true);
        foreach (var dd in dropdowns)
        {
            dd.value = data.GetConnection(i);
            dd.choices = LBSTags.GetInstance("WFC Tags").Alls;
            int n = i;
            dd.RegisterCallback<ChangeEvent<string>>(e => {
                TileEditWindow.selected.SetConnection(n, e.newValue);
                EditorUtility.SetDirty(data);
            });
            i++;
        }
        weight.value = data.weight;
        weight.RegisterCallback<ChangeEvent<string>>( e => 
        {
            TileEditWindow.selected.weight = weight.value;
        });


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
                //buttons.ForEach(b => b.style.backgroundColor =);
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
