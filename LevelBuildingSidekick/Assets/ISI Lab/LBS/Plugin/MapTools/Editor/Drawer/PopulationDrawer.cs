using ISILab.Extensions;
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
        private PopulationTileView lastHighlight = null;
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // Get behaviour
            var population = target as PopulationBehaviour;
            if (population == null)
            {
                return;
            }

            // Get rotated selection
            TileBundleGroup rotationHighlightedTile = null;
            var manipulator = ToolKit.Instance.GetActiveManipulator();
            if (manipulator is RotatePopulationTile { Selected: not null } rotate)
            {
                rotationHighlightedTile = rotate.Selected;
            }

            foreach (var nTileGroup in population.RetrieveNewRotations())
            {
                view.GetElement(population.OwnerLayer, nTileGroup).Rotate();
                
                // Check for rotation manipulator highlight
                if (rotationHighlightedTile != null && Equals(nTileGroup, rotationHighlightedTile))
                {
                    if (lastHighlight != null)
                    {
                        lastHighlight.Highlight(false);
                    }

                    foreach (var element in view.GetElement(population.OwnerLayer, rotationHighlightedTile))
                    {
                        (element as PopulationTileView).Highlight(true);
                    }
                } 
            }

            foreach (TileBundleGroup tileBundleGroup in population.Tilemap)
            {
                foreach (var nTile in population.RetrieveNewTiles())
                {
                    if (!tileBundleGroup.TileGroup.Contains(nTile)) continue;
                    
                    // Create new graph element for the tile
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
                    view.AddElement(population.OwnerLayer, tileBundleGroup, tileView);
                }
            }

            var layer = population.OwnerLayer;
            var assistant = LBSLayerHelper.GetObjectFromLayer<AssistantMapElite>(layer);
            if (assistant == null) return;
            
        }
    }
}