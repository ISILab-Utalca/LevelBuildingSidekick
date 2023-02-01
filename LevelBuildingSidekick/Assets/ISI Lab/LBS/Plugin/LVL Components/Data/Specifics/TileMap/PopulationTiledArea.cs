using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class PopulationTiledArea : TiledArea
{
    [SerializeField, JsonRequired]
    private string label = ""; // "ID" or "name"

    [SerializeField, JsonRequired]
    private Texture2D icon = null;
    [JsonIgnore]
    public string Label => label;
    [JsonIgnore]
    public Texture2D Icon => icon;

    public override object Clone()
    {
        return new PopulationTiledArea
        {
            icon = Icon,
            label = Label
        };
    }

    public void findScript(string ID)
    {
        PopulationBundle[] bundles = Resources.FindObjectsOfTypeAll<PopulationBundle>();
        PopulationBundle target = null;

        foreach (PopulationBundle bundle in bundles)
        {
            if (bundle.Label == ID)
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
