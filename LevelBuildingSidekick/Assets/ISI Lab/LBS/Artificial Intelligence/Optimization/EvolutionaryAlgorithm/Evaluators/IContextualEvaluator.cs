using Commons.Optimization.Evaluator;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IContextualEvaluator : IEvaluator
{
    public List<LBSLayer> ContextLayers { get; set; }

    public LBSLayer CombinedInteriorLayer { get; set; }

    public void InitializeDefaultWithContext(List<LBSLayer> contextLayers, Rect selection);

    public LBSLayer InteriorLayers(Rect selection)
    {
        if (ContextLayers.Count == 0) return null;

        //If there's no schema layers, return null again. This might be the last failsafe we need!
        var interiorLayers = ContextLayers.FindAll(l => l.Behaviours.Any(b => b.GetType().Equals(typeof(SchemaBehaviour))));
        if (interiorLayers.Count == 0) return null;
        
        //Clone first
        var combinedLayer = interiorLayers.First().Clone() as LBSLayer;

        // Get important modules
        var combinedSectorizedTM = combinedLayer.GetModule<SectorizedTileMapModule>();
        var combinedConnectedTM = combinedLayer.GetModule<ConnectedTileMapModule>();

        //Now we check everything
        foreach (LBSLayer interiorLayer in interiorLayers)
        {
            //Skip the one we just cloned
            if (interiorLayer.Equals(combinedLayer)) continue;
            //Get behaviors
            var tempBehavior = interiorLayer.Behaviours.Find(b => b.GetType().Equals(typeof(SchemaBehaviour))) as SchemaBehaviour;
            var combinedBehavior = combinedLayer.Behaviours.Find(b => b.GetType().Equals(typeof(SchemaBehaviour))) as SchemaBehaviour;

            // Get current layer Connections
            var currentConnectedTM = interiorLayer.GetModule<ConnectedTileMapModule>();

            foreach (LBSTile tile in tempBehavior.Tiles) 
            {
                //If there's already a tile here, return. We'll ignore everything that overlaps.
                //The warning is there for a reason!
                if (combinedBehavior.GetTile(tile.Position) != null) continue;
                //Get zone from the tile we're looking at
                var zone = tempBehavior.GetZone(tile);
                //If the zone isn't in the tilemap, add it
                if(!combinedBehavior.Zones.Contains(zone))
                {
                    //combinedBehavior.Zones.Add(zone);
                    combinedSectorizedTM.AddZone(zone);
                }
                //Then add the tile
                combinedBehavior.AddTile(tile.Position, zone);
                combinedConnectedTM.AddPair(tile, currentConnectedTM.GetConnections(tile), currentConnectedTM.GetPair(tile).EditedByIA);
            }
        }

        combinedSectorizedTM.RecalculateZonesProximity(selection);
       
        return combinedLayer;
    }

    public LBSLayer PopulationLayers()
    {
        if (ContextLayers.Count == 0) return null;

        //If there's no population layers, return null
        var populationLayers = ContextLayers.FindAll(l => l.Behaviours.Any(b => b.GetType().Equals(typeof(PopulationBehaviour))));
        if (populationLayers.Count == 0) return null;

        //Clone first
        var combinedLayer = populationLayers.First().Clone() as LBSLayer;

        //Now we check everything
        foreach (LBSLayer populationLayer in populationLayers)
        {
            //Skip the one we just cloned
            if (populationLayer.Equals(combinedLayer)) continue;
            //Get behaviors
            var tempBehavior = populationLayer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;
            var combinedBehavior = combinedLayer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;

            foreach (TileBundleGroup group in tempBehavior.Tilemap)
            {
                var tileCheck = new List<TileBundleGroup>();
                //Check every tile in the group and see if there's anything in their locations.
                foreach (LBSTile tile in group.TileGroup)
                {
                    var foundTile = combinedBehavior.GetTileGroup(tile.Position);
                    if (foundTile != null)
                    {
                        tileCheck.Add(foundTile);
                    }
                }
                //If there's anything in the tileCheck list, we skip. We don't wanna delete anything!
                if (tileCheck.Count > 0) continue;

                //Lastly, add the tile
                combinedBehavior.BundleTilemap.AddGroup(group);
            }
        }

        return combinedLayer;
    }
}
