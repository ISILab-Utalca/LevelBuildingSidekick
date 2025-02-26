using ISILab.Commons;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Modules;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class PopulationTileView : GraphElement
    {
        private static VisualTreeAsset view;

        //List<VisualElement> arrows = new List<VisualElement>();

        VisualElement icon;
        VisualElement bg;

        public PopulationTileView(TileBundleGroup tile)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationTile");
            }
            view.CloneTree(this);

            //arrows.Add(this.Q<VisualElement>(name: "Right"));
            //arrows.Add(this.Q<VisualElement>(name: "Up"));
            //arrows.Add(this.Q<VisualElement>(name: "Left"));
            //arrows.Add(this.Q<VisualElement>(name: "Down"));

            icon = this.Q<VisualElement>(name: "Icon");
            bg = this.Q<VisualElement>(name: "Background");

            var id = tile.BundleData.Bundle;
            SetColor(id.Color);
            SetImage(id.Icon);
            //SetDirection(tile.Rotation);

        }

        public void SetDirection(Vector2 vector)
        {
            var dir = Directions.Bidimencional.Edges.Select((d, i) => new { d, i }).OrderBy(o => (vector - o.d).magnitude).First().i;

            //arrows.ForEach(v => v.visible = false);
            //arrows[dir].visible = true;
        }

        public void SetColor(Color color)
        {
            bg.style.backgroundColor = color;
        }

        public void SetImage(Texture2D image)
        {
            icon.style.backgroundImage = image;
        }
    }
}