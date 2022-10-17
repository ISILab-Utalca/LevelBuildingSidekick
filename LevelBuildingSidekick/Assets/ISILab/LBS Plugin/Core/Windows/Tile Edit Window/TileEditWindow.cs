using LBS;
using LBS.VisualElements;
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

    private RenderObjectPivot pivot;
    private GameObject selected;
    private RenderObjectView screen;

    private List<string> _paths = new List<string>();
    private List<Button> buttons = new List<Button>();

    private readonly int btnSize = 64;

    public void AddItemsToMenu(GenericMenu menu)
    {
        GUIContent content = new GUIContent("Clear prefabs refenreces");
        menu.AddItem(content, false, () =>
        {
            this.content.Clear();
            _paths = new List<string>();
            EditorPrefs.SetString("TileEditorWindow", "");
            this.content.Add(addButton);
        });
    }

    [MenuItem("ISILab/LBS plugin/Tile edit window", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<TileEditWindow>();
        window.titleContent = new GUIContent("Tile Edit Window");
    }

    private void OnInspectorUpdate()
    {
        if (rTexture == null)
            return;

        var x = (int)screen.style.width.value.value;
        rTexture.width = x;
        var y = (int)screen.style.height.value.value;
        rTexture.height = y;
    }

    private void OnDisable()
    {
        SavePrefsPaths(_paths);
    }


    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileEditWindowUXML");
        visualTree.CloneTree(root);

        this.screen = root.Q<RenderObjectView>();
        this.content = root.Q<VisualElement>("Content");
        this.addButton = root.Q<Button>("AddButton");
        addButton.clicked += () => {
            var path = EditorUtility.OpenFilePanel("Load prefab", "", "prefab");
            path = DirectoryTools.FullPathToProjectPath(path);
            _paths.Add(path);
            SavePrefsPaths(_paths);
            ActualizeView();
        };
        ActualizeView();

        if (pivot == null)
        {
            pivot = SceneView.Instantiate(pref);
        }

        var muyLejos = 99999;
        pivot.transform.position = new Vector3(muyLejos, muyLejos, muyLejos);
        pivot.hideFlags = HideFlags.HideAndDontSave;

    }

    private void SetSelected(GameObject go)
    {
        selected = go;
        pivot.SetPref(go);
    }

    private void LoadButtons(List<GameObject> objs)
    {
        foreach (var o in objs)
        {
            var img = AssetPreview.GetAssetPreview(o);
            var btn = new Button();
            btn.clicked += () => {
                var selected = o;
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
        var objs = LoadPrefsPaths();
        LoadButtons(objs);

        if(objs.Count > 0)
        {
            selected = objs[0] as GameObject;
        }
        content.Add(addButton);
    }

    private List<GameObject> LoadPrefsPaths()
    {
        var objs = new List<GameObject>();
        var x = EditorPrefs.GetString("TileEditorWindow");
        _paths = x.Split("?").ToList();

        var r = new List<string>();
        foreach (var path in _paths)
        {
            var o = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (o != null)
            {
                objs.Add(o);
            }
            else
            {
                r.Add(path);
            }
        }

        r.ForEach(rp => _paths.Remove(rp));
        return objs;
    }

    private void SavePrefsPaths(List<string> paths)
    {
        var v = "";
        foreach (var p in paths)
        {
            v += p + "?";
        }

        EditorPrefs.SetString("TileEditorWindow", v);
    }


}
