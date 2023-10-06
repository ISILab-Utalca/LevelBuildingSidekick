using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using UnityEditor.Experimental.GraphView;
using Utility;

[Drawer(typeof(SchemaBehaviour))]
public class SchemaDrawer : Drawer
{
    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        // Get behaviour
        var schema = target as SchemaBehaviour;

        // Get modules
        var tilesMod = schema.Owner.GetModule<TileMapModule>();
        var zonesMod = schema.Owner.GetModule<SectorizedTileMapModule>();
        var connectionsMod = schema.Owner.GetModule<ConnectedTileMapModule>();

        foreach (var t in tilesMod.Tiles)
        {
            var zone = zonesMod.GetZone(t);

            var conections = connectionsMod.GetConnections(t);

            var tView = GetTileView(t, zone, conections, teselationSize);

            view.AddElement(tView);
        }
    }

    public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
    {
        var schema = target as SchemaBehaviour;
        var zones = schema.Zones;

        var texture = new Texture2D((int)(sourceRect.width * teselationSize.x), (int)(sourceRect.height * teselationSize.y));

        for (int j = 0; j < texture.height; j++)
        {
            for (int i = 0; i < texture.width; i++)
            {
                texture.SetPixel(i, j, new Color(0f, 0f, 0f, 0));
            }
        }

        foreach (var z in zones)
        {
            var tiles = schema.GetTiles(z);
            var text = GetTileTexture(teselationSize, z.Color);

            foreach (var t in tiles)
            {
                if (!sourceRect.Contains(t.Position))
                    continue;
                for (int j = 0; j < teselationSize.y; j++)
                {
                    for (int i = 0; i < teselationSize.x; i++)
                    {
                        var pos = t.Position - sourceRect.position;
                        texture.SetPixel((int)(pos.x * teselationSize.x) + i, (int)(pos.y * teselationSize.y) + j, text.GetPixel(i,j));
                    }
                }
            }
        }

        //texture.MirrorY();

        texture.Apply();

        return texture;
    }

    private GraphElement GetTileView(LBSTile tile, Zone zone, List<string> connections , Vector2 teselationSize)
    {
        var pos = new Vector2(tile.Position.x, -tile.Position.y);
        var tView = new SchemaTileView();
        var size = DefalutSize * teselationSize;
        tView.SetPosition(new Rect(pos * size, size));

        tView.SetBackgroundColor(zone.Color);

        tView.SetConnections(connections.ToArray());
        return tView;
    }

    private Texture2D GetTileTexture(Vector2Int size, Color color)
    {
        var t = new Texture2D(size.x, size.y);

        for (int j = 0; j < size.y; j++)
        {
            for (int i = 0; i < size.x; i++)
            {
                t.SetPixel(i, j, color);
            }
        }

        return t;
    }
}
