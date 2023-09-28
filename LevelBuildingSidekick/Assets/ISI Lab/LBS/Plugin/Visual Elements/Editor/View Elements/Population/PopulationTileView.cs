using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class PopulationTileView : GraphElement
{
    List<VisualElement> arrows = new List<VisualElement>();

    VisualElement icon;

    private static VisualTreeAsset view;

    public PopulationTileView(TileBundlePair tile)
    {
        if (view == null)
        {
            PopulationTileView.view = DirectoryTools.SearchAssetByName<VisualTreeAsset>("PopulationTile");
            //PopulationTileView.view = LBSAssetsStorage.Instance.Get<VisualTreeAsset>().Find(v => v.name == "PopulationTile");
        }
        PopulationTileView.view.CloneTree(this);

        arrows.Add(this.Q<VisualElement>(name: "Right"));
        arrows.Add(this.Q<VisualElement>(name: "Up")); 
        arrows.Add(this.Q<VisualElement>(name: "Left"));
        arrows.Add(this.Q<VisualElement>(name: "Down")); 

        icon = this.Q<VisualElement>(name: "Icon");

        var id = tile.BundleData.Bundle;
        SetColor(id.Color);
        SetImage(id.Icon);
        SetDirection(tile.Rotation);

    }

    public void SetDirection(Vector2 vector)
    {
        var dir = Directions.Bidimencional.Edges.Select((d, i) => new { d, i }).OrderBy(o => (vector - o.d).magnitude).First().i;

        arrows.ForEach(v => v.visible = false);
        arrows[dir].visible = true;
    }

    public void SetColor(Color color)
    {
        icon.style.backgroundColor = color;
    }

    public void SetImage(Texture2D image)
    {
        icon.style.backgroundImage = image;
    }
}
