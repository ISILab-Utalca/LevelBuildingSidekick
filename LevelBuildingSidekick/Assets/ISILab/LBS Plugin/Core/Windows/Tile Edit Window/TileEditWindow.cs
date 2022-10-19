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
    private DropdownField[] dropdowns = new DropdownField[4];
    private Slider distance;
    private Slider rotation;

    private RenderObjectPivot pivot;
    private TileConections selected;
    private RenderObjectView screen;

    //private List<string> _paths = new List<string>();
    private List<Button> buttons = new List<Button>();

    private readonly int btnSize = 64;

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
        });
    }

    private void OnInspectorUpdate()
    {
        if (rTexture == null)
            return;

        //var x = (int)screen.style.width.value.value;
        //rTexture.width = x;
        //var y = (int)screen.style.height.value.value;
        //rTexture.height = y;
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

        this.screen = root.Q<RenderObjectView>();
        this.content = root.Q<VisualElement>("Content");
        
        this.addButton = root.Q<Button>("AddButton");
        addButton.clicked += AddButton;
        this.distance = root.Q<Slider>("Distance");
        distance.RegisterValueChangedCallback(DistanceCamera);
        this.rotation = root.Q<Slider>("Rotation");
        rotation.RegisterValueChangedCallback(RotateCamera);

        ActualizeView();

        if (pivot == null)
        {
            pref.hideFlags = HideFlags.None;
            pivot = SceneView.Instantiate(pref);
            var muyLejos = 9999;
            pivot.transform.position = new Vector3(muyLejos, muyLejos, muyLejos);
            pivot.hideFlags = HideFlags.HideAndDontSave;
            pivot.cam.cameraType = CameraType.Preview;
        }

        SceneViewCameraWindow.additionalSettingsGui += test;
    }

    private void test(SceneView sv)
    {
        sv.ShowAuxWindow();
    }

    private void RotateCamera(ChangeEvent<float> value)
    {
        pivot.SetRotateCam(value.newValue);
        
    }

    private void DistanceCamera(ChangeEvent<float> value)
    {
        pivot.SetDistanceCam(value.newValue);
    }

    private void AddButton()
    {
        var path = EditorUtility.OpenFilePanel("Load prefab", "", "prefab");
        path = DirectoryTools.FullPathToProjectPath(path);

        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var so = ScriptableObject.CreateInstance<TileConections>();
        so.Init(go);

        var sPath = EditorUtility.SaveFilePanel("Sabe metadata","", "","asset");
        sPath = DirectoryTools.FullPathToProjectPath(sPath);
        AssetDatabase.CreateAsset(so, sPath);
        AssetDatabase.SaveAssets();

        ActualizeView();
    }

    private void SetSelected(TileConections data)
    {
        this.selected = data;
        pivot.SetPref(selected.Tile);
        var i = 0;
        foreach (var dd in dropdowns)
        {
            dd.value = data.GetConnection(i);
            dd.choices = LBSTags.GetInstance("WFC Tags").Alls;
            int n = i;
            dd.RegisterCallback<ChangeEvent<string>>(e => {
                this.selected.SetConnection(n, e.newValue);
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
