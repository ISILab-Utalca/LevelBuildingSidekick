using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private VectorImage _doorConImage = null;
        private VectorImage _windowConImage = null;
        private bool _loaded = false;
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // Get behaviour
            var schema = target as SchemaBehaviour;

            // Get modules
            var tilesMod = schema.OwnerLayer.GetModule<TileMapModule>();
            var zonesMod = schema.OwnerLayer.GetModule<SectorizedTileMapModule>();
            var connectionsMod = schema.OwnerLayer.GetModule<ConnectedTileMapModule>();

            foreach (LBSTile newTile in schema.RetrieveNewTiles())
            {
                TileZonePair tz = zonesMod.GetPairTile(newTile);
                TileConnectionsPair tc = connectionsMod.GetPair(newTile);
                
                var tView = GetTileView(newTile, tz.Zone, tc.Connections, teselationSize);
                // Stores using LBSTile as key
                view.AddElement(schema.OwnerLayer, newTile, tView);
            }
            
            // Paint all tiles
            if (!_loaded)
            {
                foreach (var tile in tilesMod.Tiles)
                {
                    TileZonePair tz = zonesMod.GetPairTile(tile);
                    TileConnectionsPair tc = connectionsMod.GetPair(tile);
                
                    var tView = GetTileView(tile, tz.Zone, tc.Connections, teselationSize);
                    // Stores using LBSTile as key
                    view.AddElement(schema.OwnerLayer, tile, tView);
                }

                _loaded = true;
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
            var tempSchemaBehaviour = new SchemaBehaviour(ScriptableObject.CreateInstance<VectorImage>(), "temp", Color.clear);
            
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
                        setIcon = GetDoorImage();
                    else if (connectionType == connectionTypes[3])
                        setIcon = GetWindowImage();

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

        private VectorImage GetDoorImage()
        {
            if (_doorConImage == null)
            {
                _doorConImage = Resources.Load<VectorImage>("Icons/Vectorial/Icon=DoorConnection");
            }
            return _doorConImage;
        }
        private VectorImage GetWindowImage()
        {
            if (_windowConImage == null)
            {
                _windowConImage = Resources.Load<VectorImage>("Icons/Vectorial/Icon=WindowsConnection");
            }
            return _windowConImage;
        }
    }
}