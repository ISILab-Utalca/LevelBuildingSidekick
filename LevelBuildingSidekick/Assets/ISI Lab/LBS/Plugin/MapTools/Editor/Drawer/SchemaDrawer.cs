using ISILab.Commons.VisualElements;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(SchemaBehaviour))]
    public class SchemaDrawer : Drawer
    {
        private VectorImage _doorConImage = null;
        private VectorImage _windowConImage = null;
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // Get behaviour
            var schema = target as SchemaBehaviour;

            // Get modules
            var tilesMod = schema.OwnerLayer.GetModule<TileMapModule>();
            var zonesMod = schema.OwnerLayer.GetModule<SectorizedTileMapModule>();
            var connectionsMod = schema.OwnerLayer.GetModule<ConnectedTileMapModule>();

            PaintNewTiles(schema, teselationSize, view, zonesMod, connectionsMod);
            UpdateLoadedTiles(schema, teselationSize, view, zonesMod, connectionsMod);
            if (!Loaded)
            {
                LoadAllTiles(schema, teselationSize, view, tilesMod, zonesMod, connectionsMod);
                Loaded = true;
            }
        }

        private void PaintNewTiles(SchemaBehaviour schema, Vector2 teselationSize, MainView view,
            SectorizedTileMapModule zonesMod, ConnectedTileMapModule connectionsMod)
        {
            foreach (LBSTile newTile in schema.RetrieveNewTiles())
            {
                TileZonePair tz = zonesMod.GetPairTile(newTile);
                TileConnectionsPair tc = connectionsMod.GetPair(newTile);
                
                var tView = GetTileView(newTile, tz.Zone, tc.Connections, teselationSize);
                
                // Stores using LBSTile as key
                view.AddElementToLayerContainer(schema.OwnerLayer, newTile, tView);
            }
        }

        private void UpdateLoadedTiles(SchemaBehaviour schema, Vector2 teselationSize, MainView view,
            SectorizedTileMapModule zonesMod, ConnectedTileMapModule connectMod)
        {
            schema.Keys.RemoveWhere(item => item == null);
            
            // Update stored tiles
            foreach (object obj in schema.Keys)
            {
                if(obj is not LBSTile tile) continue;

                var elements = view.GetElementsFromLayerContainer(schema.OwnerLayer, tile);
                if(elements == null) continue;
                
                foreach (var graphElement in elements)
                {
                    if (graphElement is not SchemaTileView tView) continue;
                    if (!tView.visible) continue;
                    
                    TileZonePair tz = zonesMod.GetPairTile(tile);
                    var connections = connectMod.GetConnections(tile);
                    if (tz == null || connections == null)
                    {
                        Debug.LogWarning("SchemaDrawer: TileZonePair or connections not found fot tile " + tile);
                        continue;
                    }
                    
                    UpdateTileView(tView, tile, tz.Zone, connections, teselationSize, schema.OwnerLayer.index);
                }
            }
        }
        
        private void UpdateTileView(SchemaTileView tView, LBSTile tile, Zone zone, List<string> connections, Vector2 teselationSize, int layerIndex)
        {
            AdjustTileView(tView, tile, zone, connections, teselationSize);
            tView.layer = layerIndex;
        }

        private void LoadAllTiles(SchemaBehaviour schema, Vector2 teselationSize, MainView view, 
            TileMapModule tilesMod, SectorizedTileMapModule zonesMod, ConnectedTileMapModule connectionsMod)
        {
            // Paint all tiles
            foreach (var tile in tilesMod.Tiles)
            {
                TileZonePair tz = zonesMod.GetPairTile(tile);
                TileConnectionsPair tc = connectionsMod.GetPair(tile);
                
                var tView = GetTileView(tile, tz.Zone, tc.Connections, teselationSize);
                // Stores using LBSTile as key
                view.AddElementToLayerContainer(schema.OwnerLayer, tile, tView);
                schema.Keys.Add(tile);
            }
        }

        public override void ShowVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not SchemaBehaviour schema) return;
            
            foreach (LBSTile tile in schema.Keys)
            {
                foreach (var graphElement in view.GetElementsFromLayerContainer(schema.OwnerLayer, tile).Where(graphElement => graphElement != null))
                {
                    graphElement.style.display = DisplayStyle.Flex;
                }
            }
        }
        public override void HideVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not SchemaBehaviour schema) return;
            
            foreach (LBSTile tile in schema.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElementsFromLayerContainer(schema.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
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
            var tView = new SchemaTileView();
            AdjustTileView(tView, tile, zone, connections, teselationSize);
            return tView;
        }

        private void AdjustTileView(SchemaTileView tView, LBSTile tile, Zone zone, List<string> connections, Vector2 teselationSize)
        {
            var pos = new Vector2(tile.Position.x, -tile.Position.y);
            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(pos * size, size));
            tView.SetBackgroundColor(zone.Color);
            tView.SetBorderColor(zone.Color, zone.BorderThickness);
            tView.SetConnections(connections.ToArray());

            tView.RemoveConnectionViews();

            var Connections = SchemaTileView.GetConnectionPoints(connections);

            foreach (var connection in Connections)
            {
                if (string.IsNullOrEmpty(connection.Key) || string.IsNullOrEmpty(connection.Value)) continue;

                // divided by 4 offsets to center the background image
                var xOffset = tView.GetPosition().width / 8f;
                var yOffset = tView.GetPosition().height / 8f;

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
                var connectionTypes = SchemaBehaviour.Connections;
                // Draw connection tile only if its not wall or open
                if (connectionType != connectionTypes[0] && connectionType != connectionTypes[1])
                {
                    VectorImage setIcon = null;
                    if (connectionType == connectionTypes[2])
                    {
                        setIcon = GetDoorImage();
                        tView.CreateConnectionView(setIcon, new Vector2(xPos, yPos), connection.Key);
                    }
                    else if (connectionType == connectionTypes[3])
                    {
                        setIcon = GetWindowImage();
                        tView.CreateConnectionView(setIcon, new Vector2(xPos, yPos), connection.Key);
                    }
                }
            }
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