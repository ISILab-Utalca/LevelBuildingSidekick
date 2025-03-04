using System;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.TileMap;
using UnityEditor.Experimental.GraphView;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.LBS.Behaviours;
using ISILab.Commons.VisualElements;
using ISILab.LBS.VisualElements;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers
{
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
                            texture.SetPixel((int)(pos.x * teselationSize.x) + i, (int)(pos.y * teselationSize.y) + j, text.GetPixel(i, j));
                        }
                    }
                }
            }

            texture.Apply();

            return texture;
        }

        private GraphElement GetTileView(LBSTile tile, Zone zone, List<string> connections, Vector2 teselationSize)
        {
            var pos = new Vector2(tile.Position.x, -tile.Position.y);
            var tView = new SchemaTileView();
            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(pos * size, size));

            tView.SetBackgroundColor(zone.Color);

            tView.SetBorderColor(zone.Color, zone.BorderThickness);
            
            tView.SetConnections(connections.ToArray());

            var Connections = SchemaTileView.GetConnectionPoints(connections);
            var tempSchemaBehaviour = new SchemaBehaviour(new Texture2D(0,0), "temp");
            
            foreach (var connection in Connections)
            {
                if (string.IsNullOrEmpty(connection.Key) || string.IsNullOrEmpty(connection.Value)) continue; 

                // divided by 4 offsets to center the background image
                var xOffset = tView.GetPosition().width/8f;
                var yOffset = tView.GetPosition().height/8f;

                float xPos = xOffset;
                float yPos = yOffset;
                
                
                switch (connection.Key)
                {
                    case "top":
                        yPos += -(tView.GetPosition().height / 2f);
                        break;
                    case "bottom":
                        yPos += (tView.GetPosition().height / 2f);
                        break;
                    case "left":
                        xPos += -(tView.GetPosition().width / 2f);
                        break;
                    case "right":
                        xPos += (tView.GetPosition().width / 2f);
                        break;
                }
                string connectionType = connection.Value;
                var connectionTypes = tempSchemaBehaviour.Connections;
                // Draw connection tile only if its not wall or open
                if (connectionType != connectionTypes[0] && connectionType != connectionTypes[1])
                {
                    VectorImage setIcon = null;
                    if (connectionType == connectionTypes[2])
                        setIcon = Resources.Load<VectorImage>("Icons/Vectorial/Icon=DoorConnection");
                    else if (connectionType == connectionTypes[3])
                        setIcon = Resources.Load<VectorImage>("Icons/Vectorial/Icon=WindowsConnection");

                    var connectionPoint = new SchemaTileConnectionView(setIcon)
                    {
                        style =
                        {
                            width = 64,
                            height = 64,
                            backgroundColor = Color.clear,
                            position = Position.Absolute,
                            left = xPos-2.5f,
                            top = yPos-2.5f
                        }
                    };
                    tView.Add(connectionPoint);
                }
            }

            
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
}