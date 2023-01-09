using LBS.Representation;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(WFCWindow), ID, "WFC Tools Extra", "-", defaultDisplay = true)]
public class WFCExtraTools : Overlay
{
    private const string ID = "WFCExtraToolsOverlay";

    private Button fillButton;
    private Button SelectToColapseButton;
    private Button ColapseButton;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("WFCExtraTools");
        visualTree.CloneTree(root);

        var window = EditorWindow.GetWindow<WFCWindow>();
        var controller = window.GetController<WFCController>();
        var data = controller.GetData() as LBSTileMapData;

        fillButton = root.Q<Button>("FillButton");
        fillButton.clicked += () =>
        {
            Fill(data);
        };

        SelectToColapseButton = root.Q<Button>("SelectToColapseButton");
        SelectToColapseButton.clicked += () =>
        {
            //var manipulator = new 
        };

        ColapseButton = root.Q<Button>("ColapseButton");
        ColapseButton.clicked += () =>
        {

        };

        return root;
    }

    public void Fill(LBSTileMapData data)
    {
        var rect = data.GetRect();

        for (int i = rect.xMin; i < rect.xMax; i++)
        {
            for (int j = rect.yMin; j < rect.yMax; j++)
            {
                var pos = new Vector2Int(i, j);

                if (data.GetTile(pos) != null)
                    continue;

                var cs = new string[4] { "", "", "", "" };
                data.AddTile(new TileData(pos, 0, cs));
            }
        }
    }
}