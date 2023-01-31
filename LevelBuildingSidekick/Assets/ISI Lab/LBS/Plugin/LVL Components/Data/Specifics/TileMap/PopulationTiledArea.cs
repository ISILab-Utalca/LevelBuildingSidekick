using LBS.Components.TileMap;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationTiledArea<T> : TileMapModule<T> where T : LBSTile
{
    public List<PopulationBundle> bundles = new List<PopulationBundle>();
    public void AddTilesToBundle(List<T> tiles)
    {
        if (bundles.Count == 0)
        {
            bundles.Add(ScriptableObject.CreateInstance<PopulationBundle>());
        }

        // Add the tiles to the first bundle in the list
        PopulationBundle bundle = bundles[0];
        foreach (T tile in tiles)
        {
            //bundle.objects.Add(tile);
            //bundle.tags.Add(new LBSTag(tile.name));
        }
    }

    public void RemoveTileFromBundle(T tile)
    {
        // Find the bundle that contains the tile
        LBSTagsBundle bundle = bundles.Find(b => b.Contains(tile));
        if (bundle == null)
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
    }
        */
}