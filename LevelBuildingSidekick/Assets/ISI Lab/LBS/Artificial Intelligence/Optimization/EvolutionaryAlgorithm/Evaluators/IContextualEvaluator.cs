using Commons.Optimization.Evaluator;
using ISILab.Commons;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
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

    public LBSLayer CombinedLayer { get; set; }

    public LBSLayer CombinedInteriorLayer { get; set; }
    public LBSLayer CombinedExteriorLayer { get; set; }

    public void InitializeDefaultWithContext(List<LBSLayer> contextLayers, Rect selection);

    public LBSLayer InteriorLayers(Rect selection)
    {
        if (ContextLayers.Count == 0) return null;

        //If there's no schema layers, return null again. This might be the last failsafe we need!
        var interiorLayers = ContextLayers.FindAll(l => l.Behaviours.Any(b => b.GetType().Equals(typeof(SchemaBehaviour))));
        if (interiorLayers.Count == 0) return null;
        
        //Clone first
        var combinedLayer = interiorLayers[0].Clone() as LBSLayer;

        if(interiorLayers.Count == 1) return combinedLayer;

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

        //combinedSectorizedTM.RecalculateZonesProximity(selection);
       
        return combinedLayer;
    }

    public LBSLayer ExteriorLayers(Rect selection)
    {
        if (ContextLayers.Count == 0) return null;

        List<LBSLayer> exteriorLayers = ContextLayers.FindAll(l => l.ID.Equals("Exterior"));
        if (exteriorLayers.Count == 0) return null;

        //return exteriorLayers[0];

        var combinedLayer = exteriorLayers[0].Clone() as LBSLayer;

        if (exteriorLayers.Count == 1) return combinedLayer;

        var combinedSectorTM = combinedLayer.GetModule<SectorizedTileMapModule>();
        var combinedConnectedTM = combinedLayer.GetModule<ConnectedTileMapModule>("TempConnectedModule");

        foreach(LBSLayer exteriorLayer in exteriorLayers)
        {
            if (exteriorLayer.Equals(combinedLayer)) continue;

            //var tempBehaviour = exteriorLayer.Behaviours.Find(b => b.GetType().Equals(typeof(ExteriorBehaviour))) as ExteriorBehaviour;
            var combinedBehaviour = combinedLayer.Behaviours.Find(b => b.GetType().Equals(typeof(ExteriorBehaviour))) as ExteriorBehaviour;

            var currentSectorTM = exteriorLayer.GetModule<SectorizedTileMapModule>();
            var currentConnectedTM = exteriorLayer.GetModule<ConnectedTileMapModule>("TempConnectedModule");

            foreach(TileZonePair pair in currentSectorTM.PairTiles)
            {
                LBSTile tile = pair.Tile;
                //if (combinedBehaviour.GetTile(tile.Position) is not null) continue;
                if (combinedSectorTM.GetPairTile(tile) is not null) continue;
                var zone = currentSectorTM.PairTiles.Find(tzp => tzp.Tile.Equals(tile)).Zone;
                    //.Zones.Find(z => z.Positions.Contains(tile.Position));
                if(!combinedSectorTM.Zones.Contains(zone))
                {
                    combinedSectorTM.AddZone(zone);
                }
                combinedSectorTM.AddTile(tile, zone);
                combinedConnectedTM.AddPair(tile, currentConnectedTM.GetConnections(tile), currentConnectedTM.GetPair(tile).EditedByIA);
                combinedLayer.GetModule<TileMapModule>().AddTile(tile);
            }
        }
        
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

    // TODO: Handle overlap
    public LBSLayer MergeExteriorWithInterior(LBSLayer exteriorLayer, LBSLayer interiorLayer, Rect selection)
    {
        bool interiorExists = interiorLayer is not null;
        bool exteriorExists = exteriorLayer is not null;

        if (!(interiorExists || exteriorExists)) return null;

        if (!exteriorExists)
        {
            interiorLayer.GetModule<SectorizedTileMapModule>().RecalculateZonesProximity(selection);
            return interiorLayer;
        }
        if (!interiorExists)
        {
            exteriorLayer.GetModule<SectorizedTileMapModule>().RecalculateZonesProximity(selection, exteriorLayer.GetModule<ConnectedTileMapModule>("TempConnectedModule"));
            return exteriorLayer;
        }

        LBSLayer newLayer = exteriorLayer.Clone() as LBSLayer;

        var exteriorSectorTM = newLayer.GetModule<SectorizedTileMapModule>();
        var exteriorZonesConnectedTM = newLayer.GetModule<ConnectedTileMapModule>("TempConnectedModule");
        var exteriorTilemap = newLayer.GetModule<TileMapModule>();
        if (exteriorSectorTM is null || exteriorZonesConnectedTM is null || exteriorTilemap is null)
        {
            Debug.Log($"Exterior Layer '{exteriorLayer.Name}' not suitable for MAP Elites Context.");
            interiorLayer.GetModule<SectorizedTileMapModule>().RecalculateZonesProximity(selection);
            return interiorLayer;
        }

        var interiorSectorTM = interiorLayer.GetModule<SectorizedTileMapModule>();
        var interiorConnectedTM = interiorLayer.GetModule<ConnectedTileMapModule>();

        foreach(LBSTile tile in interiorSectorTM.PairTiles.Select(t => t.Tile))
        {
            if (exteriorSectorTM.PairTiles.Any(tzp => tzp.Tile.Equals(tile)))
            {
                exteriorSectorTM.RemovePair(tile);
                exteriorZonesConnectedTM.RemoveTile(tile);
                exteriorTilemap.RemoveTile(tile);
                //continue;
            }

            Zone zone = interiorSectorTM.PairTiles.Find(tzp => tzp.Tile.Position.Equals(tile.Position)).Zone;
            if (!exteriorSectorTM.Zones.Contains(zone))
            {
                exteriorSectorTM.AddZone(zone);
            }
            
            exteriorTilemap.AddTile(tile);
            exteriorSectorTM.AddTile(tile, zone);
            exteriorZonesConnectedTM.AddPair(tile, interiorConnectedTM.GetConnections(tile), new List<bool>() { false, false, false, false });
            
            foreach(var dir in interiorConnectedTM.GetPair(tile).HasConnections("Door"))
            {
                TileConnectionsPair other = exteriorZonesConnectedTM.GetPair(tile.Position + Directions.Bidimencional.Edges[dir]);
                if(other is null) continue;
                other.SetConnection((dir + 2) % 4, "Door", false);
            }
        }

        exteriorSectorTM.RecalculateZonesProximity(selection, exteriorZonesConnectedTM);
        
        return newLayer;
    }
}
