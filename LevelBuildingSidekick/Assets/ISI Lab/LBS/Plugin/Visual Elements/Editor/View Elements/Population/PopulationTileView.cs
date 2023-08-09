using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulationTileView : GraphElement
{
    List<VisualElement> arrows;

    VisualElement icon;

    public PopulationTileView(TileBundlePair tile)
    {
        var visualTreeAsset = LBSAssetsStorage.Instance.Get<VisualTreeAsset>().Find(v => v.name == "PopulationTile");
        visualTreeAsset.CloneTree(this);

        arrows.Add(this.Q<VisualElement>(name: "Right"));
        arrows.Add(this.Q<VisualElement>(name: "Up")); 
        arrows.Add(this.Q<VisualElement>(name: "Left"));
        arrows.Add(this.Q<VisualElement>(name: "Down")); 

        icon = this.Q<VisualElement>(name: "Icon");

        var id = tile.BundleData.Identifier;
        SetColor(id.Color);
        SetImage(id.Icon);
        SetDirection(tile.BundleData.GetCharasteristic<LBSRotationCharacteristic>().Rotation);

    }

    public void SetDirection(Vector2 vector)
    {
        var dir = Directions.Bidimencional.Edges.Select((d, i) => new { d, i }).OrderBy(o => (vector - o.d).magnitude).First().i;

        arrows.ForEach(v => v.SetDisplay(false));
        arrows[0].SetDisplay(true);
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
