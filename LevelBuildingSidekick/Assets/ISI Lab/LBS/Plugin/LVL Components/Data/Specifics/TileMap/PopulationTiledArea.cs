using LBS.Components.TileMap;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class PopulationTiledArea : TileMapModule<PopulationTiles>
{
    [SerializeField, JsonRequired]
    private string label = ""; // "ID" or "name"
    public void findScript(string ID)
    {
        PopulationBundle[] bundles = Resources.FindObjectsOfTypeAll<PopulationBundle>();
        PopulationBundle target = null;

        foreach (PopulationBundle bundle in bundles)
        {
            if (bundle.label == ID)
            {
                target = bundle;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogError("No bundle with ID " + ID + " was found");
            return;
        }

        List<GameObject> objects = target.GetObjects();

        // Do something with the found objects
    }

}