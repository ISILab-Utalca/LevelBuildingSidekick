using LBS.Components.TileMap;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class PopulationTiledArea : TileMapModule<PopulationTiles>
{
    [SerializeField, JsonRequired]
    private string label = ""; // "ID" or "name"

    public void AddTilesToBundle(List<PopulationTiles> tiles)
    {
        /*
        if (bundles.Count == 0)
        {
            bundles.Add(ScriptableObject.CreateInstance<PopulationBundle>());
        }
        */
        // Add the tiles to the first bundle in the list
        //PopulationBundle bundle = bundles[0];
        foreach (PopulationTiles tile in tiles)
        {
            //bundle.objects.Add(tile);
            //bundle.tags.Add(new LBSTag(tile.name));
        }
    }

    public void RemoveTileFromBundle(PopulationTiles tile)
    {
        // Find the bundle that contains the tile
        //LBSTagsBundle bundle = bundles.Find(b => b.Contains(tile));
        //if (bundle == null)
        {
            return;
        }
        /*
        // Remove the tile from the bundle
        //int index = bundle.objects.IndexOf(tile);
        if (index != -1)
        {
            bundle.objects.RemoveAt(index);
            bundle.tags.RemoveAt(index);
        }
    */
        }
        
}