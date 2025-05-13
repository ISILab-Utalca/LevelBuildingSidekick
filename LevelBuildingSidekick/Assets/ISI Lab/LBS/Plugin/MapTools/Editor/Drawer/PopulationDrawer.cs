using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using ISILab.Macros;
using LBS.VisualElements;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(PopulationBehaviour))]
    public class PopulationDrawer : Drawer
    {

        private Rect previousRect;
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var population = target as PopulationBehaviour;

            if (population == null)
            {
                return;
            }

            TileBundleGroup rotationHighlightedTile = null;
            var manipulator = ToolKit.Instance.GetActiveManipulator();
            if (manipulator is RotatePopulationTile { Selected: not null } rotate)
            {
                rotationHighlightedTile = rotate.Selected;
            }
            
            foreach (TileBundleGroup tileBundleGroup in population.Tilemap)
            {
                PopulationTileView tileView = new PopulationTileView(tileBundleGroup);
                Vector2 size = population.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
                Vector2Int bundleSize = tileBundleGroup.GetBundleSize();
                //This sets the size of the group tile to draw and seems to work. Yay!
                tileView.SetSize(size * bundleSize);

                tileView.SetPivot(new Vector2(LBSSettings.Instance.general.TileSize.x * bundleSize.x, LBSSettings.Instance.general.TileSize.y * bundleSize.y));

                tileView.Highlight(false);
                // Check for rotation manipulator highlight
                if (rotationHighlightedTile != null && Equals(tileBundleGroup, rotationHighlightedTile))
                {
                    tileView.Highlight(true);
                } 
                    
                ToolKit.Instance.GetActiveManipulator();
            
                
                Vector2 position = new Vector2(tileBundleGroup.GetBounds().x, -tileBundleGroup.GetBounds().y);
                tileView.SetPosition(new Rect(position * size, size));
                view.AddElement(population.OwnerLayer,this,tileView);
                
            }

            var layer = population.OwnerLayer;
            var assistant = LBSLayerHelper.GetObjectFromLayer<AssistantMapElite>(layer);
            if (assistant == null) return;
            
        }
    }
}