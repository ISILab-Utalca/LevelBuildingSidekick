using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[ModuleTexturizer(typeof(LBSSchema))]
public class SchemaTexturizer : ModuleTexturizer
{
    public int scale = 16;
    public override Texture2D ToTexture(LBSModule module)
    {
        var schema = module as LBSSchema;

        var rect = schema.GetRect();
        var areas = schema.Areas;

        var texture = new Texture2D(rect.width*scale, rect.height*scale);

        for (int j = 0; j < texture.height; j++)
        {
            for (int i = 0; i < texture.width; i++)
            {
                texture.SetPixel(0, 0, new Color(0, 0, 0, 0));
            }
        }

        foreach(var area in areas)
        {
            var pos = area.Rect.position;
            var tiles = area.Tiles;

            Debug.Log(area.Color);

            foreach(var t in tiles)
            {
                for (int j = 0; j < scale; j++)
                {
                    for (int i = 0; i < scale; i++)
                    {
                        texture.SetPixel((int)(pos.x + t.Position.x) * scale + i, (int)(pos.y + t.Position.y) * scale + j, area.Color);
                    }
                }
            }
        }

        texture.Apply();

        return texture;
    }
}
