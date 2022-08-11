using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class MapDataScriptable : ScriptableObject
{
    internal List<Tile> tiles = new List<Tile>();

    public Tile CreateTile(Vector2Int pos)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.position = pos;
        tile.name = "'"+this.name + "' Node"; 
        tile.guid = GUID.Generate().ToString();
        tiles.Add(tile);

        AssetDatabase.AddObjectToAsset(tile,this);
        AssetDatabase.SaveAssets();
        return tile;
    }

    public void RemoveTile(Tile tile)
    {
        tiles.Remove(tile);
        AssetDatabase.RemoveObjectFromAsset(tile);
        AssetDatabase.SaveAssets();
    }
}

