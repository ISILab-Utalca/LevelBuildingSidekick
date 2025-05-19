using ISILab.Commons;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Modules;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Characteristics;
using LBS.Bundles;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class PopulationTileView : GraphElement
    {
        private static VisualTreeAsset view;

        List<VisualElement> arrows = new List<VisualElement>();
        VisualElement main;
        VisualElement icon;
        VisualElement bg;
        
        float borderWidth = 2f;
        
        public PopulationTileView(TileBundleGroup tile)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationTile");
            }
            view.CloneTree(this);

            arrows.Add(this.Q<VisualElement>(name: "Right"));
            arrows.Add(this.Q<VisualElement>(name: "Up"));
            arrows.Add(this.Q<VisualElement>(name: "Left"));
            arrows.Add(this.Q<VisualElement>(name: "Down"));

            main = this.Q<VisualElement>(name: "Pivot");
            icon = this.Q<VisualElement>(name: "Icon");
            bg = this.Q<VisualElement>(name: "Background");

            Bundle bundle = tile.BundleData.Bundle;
            SetColor(bundle.Color);
            SetImage(bundle.Icon);
            SetDirection(tile.Rotation);
            
            if (bundle.GetHasTagCharacteristic("NonRotate"))
            {
                foreach (var arrow in arrows)
                {
                    arrow.style.display = DisplayStyle.None;
                }
            }


           // borderWidth = bg.style.borderBottomWidth.value;
        }

        public void SetDirection(Vector2 vector)
        {
            var dir = Directions.Bidimencional.Edges.Select((d, i) => new { d, i }).OrderBy(o => (vector - o.d).magnitude).First().i;

            arrows.ForEach(v => v.visible = false);
            arrows[dir].visible = true;
        }

        public void SetPivot(Vector2 pivot)
        {
            main.style.left = pivot.x;
            main.style.top = pivot.y;
        }

        public void SetSize(Vector2 vector)
        {
            main.style.width = vector.x;
            main.style.height = vector.y;

            //arrows.ForEach(v => v.visible = false);
            //arrows[dir].visible = true;
        }

        public void SetColor(Color color)
        {
            bg.style.backgroundColor = color;
        }

        public void SetImage(VectorImage icon)
        {
            this.icon.style.backgroundImage = new StyleBackground(icon);
        }

        public void Highlight(bool highlight)
        {
            var newBorderWidth = highlight ? borderWidth*3 : borderWidth;
            bg.style.borderBottomWidth = newBorderWidth;
            bg.style.borderTopWidth = newBorderWidth;
            bg.style.borderLeftWidth = newBorderWidth;
            bg.style.borderRightWidth = newBorderWidth;
        }
    }
}