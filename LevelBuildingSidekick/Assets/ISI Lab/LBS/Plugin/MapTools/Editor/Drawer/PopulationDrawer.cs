using System.Linq;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components.TileMap;
using LBS.VisualElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

            
            PaintNewTiles(population, view);
            UpdateLoadedTiles(population, view);
            
            // Paint all tiles
            if (!Loaded || FullRedrawRequested)
            {
                LoadAllTiles(population, view);
                Loaded = true;
                FullRedrawRequested = false;
            }

            var layer = population.OwnerLayer;
            var assistant = LBSLayerHelper.GetObjectFromLayer<AssistantMapElite>(layer);
            if (assistant == null) return;
        }

        private void PaintNewTiles(PopulationBehaviour population, MainView view)
        {
            // New tiles
            foreach (TileBundleGroup nTile in population.RetrieveNewTiles())
            {
                // Stores using TileBundleGroup as key
                view.AddElement(population.OwnerLayer, nTile, CreatePopulationTileView(nTile, population));
            }
        }

        private void UpdateLoadedTiles(PopulationBehaviour population, MainView view)
        {
            // Get rotated selection
            TileBundleGroup rotationHighlightedTile = null;
            var manipulator = ToolKit.Instance.GetActiveManipulator();
            if (manipulator is RotatePopulationTile { Selected: not null } rotate)
            {
                rotationHighlightedTile = rotate.Selected;
            }

            // Rotations
            foreach (var nTileGroup in population.RetrieveNewRotations())
            {
                // Get PopulationTileViews from MainView
                foreach (var graphElement in view.GetElements(population.OwnerLayer, nTileGroup))
                {
                    // Rotate visual element
                    var tView = (PopulationTileView)graphElement;
                    tView?.SetDirection(nTileGroup.Rotation);

                    // Check for rotation manipulator highlight
                    if (rotationHighlightedTile != null && Equals(nTileGroup, rotationHighlightedTile))
                    {
                        lastHighlight?.Highlight(false);
                        lastHighlight = tView;
                        lastHighlight?.Highlight(true);
                    }
                    graphElement.layer = population.OwnerLayer.index;
                }
            }

            // Update stored tiles
            foreach (TileBundleGroup tile in population.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElements(population.OwnerLayer, tile);
                if(elements == null) continue;
                
                foreach (var graphElement in elements)
                {
                    var tView = (PopulationTileView)graphElement;
                    if (tView == null) continue;
                    if (!tView.visible) continue;

                    UpdatePopulationTile(tView, tile, population);
                }
            }
        }
        
        private void UpdatePopulationTile(PopulationTileView tileView, TileBundleGroup nTile, PopulationBehaviour population)
        {
            Vector2 size = population.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
            Vector2Int bundleSize = nTile.GetBundleSize();
                
            tileView.SetSize(size * bundleSize);
            tileView.SetPivot(new Vector2(LBSSettings.Instance.general.TileSize.x * bundleSize.x, LBSSettings.Instance.general.TileSize.y * bundleSize.y));
                
            Vector2 position = new Vector2(nTile.GetBounds().x, -nTile.GetBounds().y);
            tileView.SetPosition(new Rect(position * size, size));
            
            tileView.layer = population.OwnerLayer.index;
        }

        private void LoadAllTiles(PopulationBehaviour population, MainView view)
        {
            foreach (TileBundleGroup tileBundleGroup in population.Tilemap)
            {
                // Stores using TileBundleGroup as key
                //view.AddElement(population.OwnerLayer, tileBundleGroup, CreatePopulationTileView(tileBundleGroup, population));
                //population.Keys.Add(tileBundleGroup);
                // Stores using TileBundleGroup as key
                var tileView = CreatePopulationTileView(tileBundleGroup, population);
                if (tileView != null)
                    view.AddElement(population.OwnerLayer, tileBundleGroup, tileView);
                population.Keys.Add(tileBundleGroup);
            }
        }

        public override void ShowVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not PopulationBehaviour population) return;
            
            foreach (TileBundleGroup tile in population.Keys)
            {
                foreach (var graphElement in view.GetElements(population.OwnerLayer, tile).Where(graphElement => graphElement != null))
                {
                    graphElement.style.display = DisplayStyle.Flex;
                }
            }
        }
        public override void HideVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not PopulationBehaviour population) return;
            
            foreach (TileBundleGroup tile in population.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElements(population.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
            }
        }

        private PopulationTileView CreatePopulationTileView(TileBundleGroup nTile, PopulationBehaviour population)
        {
            // Validates
            var bundle = nTile.BundleData.Bundle;

            if (bundle == null)
            {
                Debug.LogError($"Could not draw element \"{nTile.BundleData.BundleName}\". (Compatibility problem?)");
                return null;
            }


            // Create new graph element for the tile
            PopulationTileView tileView = new PopulationTileView(nTile);
                
            Vector2 size = population.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
            Vector2Int bundleSize = nTile.GetBundleSize();
                
            tileView.SetSize(size * bundleSize);
            tileView.SetPivot(new Vector2(LBSSettings.Instance.general.TileSize.x * bundleSize.x, LBSSettings.Instance.general.TileSize.y * bundleSize.y));
            tileView.Highlight(false);
                
            ToolKit.Instance.GetActiveManipulator(); // Esto hace algo?
                
            Vector2 position = new Vector2(nTile.GetBounds().x, -nTile.GetBounds().y);
            tileView.SetPosition(new Rect(position * size, size));

            return tileView; 
        }
    }
}