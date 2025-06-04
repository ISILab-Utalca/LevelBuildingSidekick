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

        private const int VisualTilesCap = 100;
        private readonly Dictionary<int, GraphElement> _storedVisualTiles = new ();
        private const int VisualZonesCap = 10;
        private readonly Dictionary<Zone, List<TrueTile>> _storedVisualZones = new ();
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // Get behaviour
            var schema = target as SchemaBehaviour;

            // Get modules
            var zonesMod = schema.OwnerLayer.GetModule<SectorizedTileMapModule>();
            var connectionsMod = schema.OwnerLayer.GetModule<ConnectedTileMapModule>();
            
            // Update zones' positions
            zonesMod.UpdateZonePositions();
            
            // Store data of changed zones
            var changedZones = schema.RetrieveChangedZones();
            foreach (var zone in changedZones)
            {
                if (zone == null) continue;
                
                _storedVisualZones.Remove(zone);
                foreach (var pos in zone.Positions)
                {
                    AddTileToZoneMemory(zone, new TrueTile(zonesMod.GetPairTile(pos), connectionsMod.GetPair(pos)));
                }
            }
            
            // Paint each zone
            foreach (var tile in _storedVisualTiles)
            {
                Debug.Log(tile.Key.GetHashCode());
            }
            
            foreach(var zone in zonesMod.Zones)
            {
                if (!_storedVisualZones.TryGetValue(zone, out List<TrueTile> trueTiles))
                {
                    if (zone.Positions.Count > 0)
                    {
                        // how did this happen?? we are smarter than this
                        Debug.LogError("Zone not found on Schema Drawer cache: " + zone.ID);   
                    }
                    continue;  
                }
                
                foreach (TrueTile tTile in trueTiles)
                {
                    //Debug.Log("trying to get graph element of: " + tTile.GetHashCode());
                    if (_storedVisualTiles.TryGetValue(tTile.GetHashCode(), out var gElement))    // get graphElement if on memory
                    {
                        var pos = new Vector2(tTile.ZoneTile.Tile.x, -tTile.ZoneTile.Tile.y);
                        var size = DefalutSize * teselationSize;
                        
                        (gElement as SchemaTileView).SetPosition(new Rect(pos * size, size));
                        
                        view.AddElement(schema.OwnerLayer, this, gElement);
                        continue;
                    }

                    var t = tTile.ZoneTile.Tile;                               // create graphElement if not
                    var connections = tTile.ConnectionTile.Connections;
                    var tView = GetTileView(t, zone, connections, teselationSize);
                
                    Debug.Log("graphElement not found for: " + tTile.GetHashCode() + ", creating new one.");
                    _storedVisualTiles.Add(tTile.GetHashCode(), tView);
                    view.AddElement(schema.OwnerLayer, this, tView);
                }
            }
            
            // Memory handling
            //RestrictMemoryUsage(_storedVisualTiles, VisualTilesCap);
            //RestrictMemoryUsage(_storedVisualZones, VisualZonesCap);
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

        private Dictionary<T,TA> RestrictMemoryUsage<T,TA>(Dictionary<T,TA> dictionary, int maxSize)
        {
            int gap = dictionary.Count - maxSize;
            for(int i = 0; i < gap; i++)
            {
                dictionary.Remove(dictionary.First().Key);
            }
            return dictionary;
        }

        private void AddTileToZoneMemory(Zone zone, TrueTile tile)
        {
            if (_storedVisualZones.TryGetValue(zone, out var visualZone))
            {
                visualZone.Add(tile);
                return;
            }
            _storedVisualZones.Add(zone, new List<TrueTile>{ tile });
        }
    }

    internal class TrueTile
    {
        public TileZonePair ZoneTile { get; }
        public TileConnectionsPair ConnectionTile { get; }

        public TrueTile(TileZonePair zoneTile, TileConnectionsPair connectionTile)
        {
            ZoneTile = zoneTile;
            ConnectionTile = connectionTile;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ZoneTile.GetHashCode(), ConnectionTile.GetHashCode());
        }
    }
}