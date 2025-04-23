using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using UnityEngine;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(PopulationBehaviour))]
    public class PopulationDrawer : Drawer
    {
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var population = target as PopulationBehaviour;

            if (population == null)
            {
                return;
            }

            foreach (var t in population.Tilemap)
            {
                var v = new PopulationTileView(t);
                var size = population.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
                var bundleSize = t.GetBundleSize();
                //This sets the size of the group tile to draw and seems to work. Yay!
                v.SetSize(size * bundleSize);

                v.SetPivot(new Vector2(LBSSettings.Instance.general.TileSize.x * bundleSize.x, LBSSettings.Instance.general.TileSize.y * bundleSize.y));
                
                var p = new Vector2(t.GetBounds().x, -t.GetBounds().y);
                v.SetPosition(new Rect(p * size, size));
                view.AddElement(population.OwnerLayer,this,v);
                
            }
        }
    }
}