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
                var size = population.Owner.TileSize * LBSSettings.Instance.general.TileSize;
                
                foreach(var tile in t.TileGroup)
                {
                    var p = new Vector2(tile.Position.x, -tile.Position.y);
                    v.SetPosition(new Rect(p * size, size));
                    view.AddElement(v);
                }
            }
        }
    }
}