using LBS.Representation;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[Overlay(typeof(WFCWindow), ID, "TileBrush", "-", defaultDisplay = true)]
public class TileBrush : Overlay
{
    private const string ID = "TilePalletOverlay";

    public static TileView selected;

    public List<TileConections> tiles = new List<TileConections>();

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TileBrush");
        visualTree.CloneTree(root);

        LoadTiles(root); // (!!) arreglar, pasar contenedor correspon diente en lugar de root

        return root;
    }

    public void LoadTiles(VisualElement container)
    {
        var _WFCTiles = DirectoryTools.GetScriptables<TileConections>().ToList();
        for (int i = 0; i < _WFCTiles.Count; i++)
        {
            var tags = _WFCTiles[i].Connections;
            if (tiles.Contains(_WFCTiles[i]))
                return;

            tiles.Add(_WFCTiles[i]);
            var view = new TileSimple(); // <- view
            view.SetView(tags);
            var btn = new Button();
            btn.style.width = btn.style.height = 32;
            btn.Add(view);
            btn.clicked += () =>
            {
                var tempView = view;
                selected = tempView;
            };
            container.Add(btn);
        }
    }

}
