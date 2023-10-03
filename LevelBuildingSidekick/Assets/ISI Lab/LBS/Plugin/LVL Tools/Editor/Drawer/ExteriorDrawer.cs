using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using static UnityEditor.Experimental.GraphView.GraphView;

[Drawer(typeof(ExteriorBehaviour))]
public class ExteriorDrawer : Drawer
{
    private List<LBSIdentifier> Identifiers => LBSAssetsStorage.Instance.Get<LBSIdentifier>();
    public ExteriorDrawer() : base() { }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        // Get behaviours
        var exterior = target as ExteriorBehaviour;

        // Get modules
        var tileMod = exterior.Owner.GetModule<TileMapModule>();
        var connectMod = exterior.Owner.GetModule<ConnectedTileMapModule>();

        foreach (var tile in exterior.Tiles)
        {
            var connections = connectMod.GetConnections(tile);

            var tView = GetTileView(tile, connections, teselationSize);

            view.AddElement(tView);
        }
    }

    public GraphElement GetTileView(LBSTile tile, List<string> connections, Vector2 teselationSize)
    {

        var tView = new ExteriorTileView(connections);
        tView.SetBackgroundColor(Color.gray);
        tView.SetConnections(connections.ToArray());

        var pos = new Vector2(tile.Position.x, -tile.Position.y);

        var size = DefalutSize * teselationSize;
        tView.SetPosition(new Rect(pos * size, size));

        return tView;
    }

    public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
    {
        var exterior = target as ExteriorBehaviour;

        var tileMod = exterior.Owner.GetModule<TileMapModule>();
        var connectMod = exterior.Owner.GetModule<ConnectedTileMapModule>();

        var texture = new Texture2D((int)(sourceRect.width * teselationSize.x), (int)(sourceRect.height * teselationSize.y));

        for (int j = 0; j < texture.height; j++)
        {
            for (int i = 0; i < texture.width; i++)
            {
                texture.SetPixel(i, j, new Color(0.0f, 0.0f, 0.0f, 0));
            }
        }

        int c = 0;
        foreach (var t in tileMod.Tiles)
        {
            if (!sourceRect.Contains(t.Position))
                continue;
            c++;
            var connections = connectMod.GetConnections(t);
            var text = GetTileTexture(connections, teselationSize);
            for (int j = 0; j < teselationSize.y; j++)
            {
                for (int i = 0; i < teselationSize.x; i++)
                {
                    var pos = t.Position - sourceRect.position;
                    texture.SetPixel((int)(pos.x * teselationSize.x) + i, (int)(pos.y * teselationSize.y) + j, text.GetPixel(i, j));
                }
            }
        }

        texture.Apply();

        return texture;
    }

    public Texture2D GetTileTexture(List<string> connections, Vector2Int teselationSize)
    {
        var texture = new Texture2D(teselationSize.x, teselationSize.y);

        for (int j = 0; j < teselationSize.y; j++)
        {
            for (int i = 0; i < teselationSize.x; i++)
            {
                //UP and DOWN Might Change on MIRROR
                if(i < teselationSize.x*0.33) // LEFT
                {
                    if (j < teselationSize.y * 0.33 || j > teselationSize.y * 0.66)
                        continue;
                    var index = Directions.Bidimencional.Edges.FindIndex(v => v == Vector2.left);
                    var connection = connections[index];
                    var color = Identifiers.Find(t => t.Label.Equals(connection)).Color;
                    texture.SetPixel(i, j, color);
                }
                else if(i > teselationSize.x*0.66) // RIGHT
                {
                    if (j < teselationSize.y * 0.33 || j > teselationSize.y * 0.66)
                        continue;
                    var index = Directions.Bidimencional.Edges.FindIndex(v => v == Vector2.right);
                    var connection = connections[index];
                    var color = Identifiers.Find(t => t.Label.Equals(connection)).Color;
                    texture.SetPixel(i, j, color);
                }
                if (j < teselationSize.y * 0.33) // DOWN
                {
                    if (i < teselationSize.x * 0.33 || i > teselationSize.x * 0.66)
                        continue;
                    var index = Directions.Bidimencional.Edges.FindIndex(v => v == Vector2.down);
                    var connection = connections[index];
                    var color = Identifiers.Find(t => t.Label.Equals(connection)).Color;
                    texture.SetPixel(i, j, color);
                }
                else if (j > teselationSize.y * 0.66) // UP
                {

                    if (i < teselationSize.x * 0.33 || i > teselationSize.x * 0.66)
                        continue;
                    var index = Directions.Bidimencional.Edges.FindIndex(v => v == Vector2.up);
                    var connection = connections[index];
                    var color = Identifiers.Find(t => t.Label.Equals(connection)).Color;
                    texture.SetPixel(i, j, color);
                }

                if((i < teselationSize.x * 0.33 || i > teselationSize.x * 0.66) && (j < teselationSize.y * 0.33 || j > teselationSize.y * 0.66))
                {
                    texture.SetPixel(i, j, Color.gray);
                }

            }
        }

        return texture;
    }
}
