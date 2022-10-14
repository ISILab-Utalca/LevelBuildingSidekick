using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TileEditWindow : EditorWindow
{
    public RenderObjectPivot pref;

    private RenderObjectPivot pivot;

    [MenuItem("ISILab/LBS plugin/Tile edit window", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<TileEditWindow>();
        window.titleContent = new GUIContent("Tile Edit Window");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileEditWindowUXML");
        visualTree.CloneTree(root);

        pivot = SceneView.Instantiate(pref);
        var muyLejos = 999999;
        pivot.transform.position = new Vector3(muyLejos, muyLejos, muyLejos);
        pivot.hideFlags = HideFlags.HideAndDontSave;

    }
}
