using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Utility;

[ModuleTexturizer(typeof(LBSSchema))]
public class SchemaTexturizer : ModuleTexturizer
{
    public int scale = 16;
    public override Texture2D ToTexture(LBSModule module)
    {
        var schema = module as LBSSchema;

        var rect = schema.GetBounds();
        var areas = schema.Areas;

        var texture = new Texture2D((int)rect.width*scale, (int)rect.height*scale);

        for (int j = 0; j < texture.height; j++)
        {
            for (int i = 0; i < texture.width; i++)
            {
                texture.SetPixel(i, j, new Color(0.1f, 0.1f, 0.1f, 1));
            }
        }

        foreach(var area in areas)
        {
            var tiles = area.Tiles;

            foreach(var t in tiles)
            {
                for (int j = 0; j < scale; j++)
                {
                    for (int i = 0; i < scale; i++)
                    {
                        texture.SetPixel((t.Position.x - (int)rect.position.x) * scale + i,  (t.Position.y - (int)rect.position.y) * scale + j, area.Color);
                    }
                }
            }
        }

        texture.MirrorY();

        texture.Apply();

        return texture;
    }
}
