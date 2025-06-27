using ISILab.Commons;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor; // TODO: Search the reference to this namespace and remove it
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(ExteriorBehaviour))]
    public class ExteriorDrawer : Drawer
    {
        private List<LBSTag> Identifiers => LBSAssetsStorage.Instance.Get<LBSTag>();
        private bool _loaded = false;
        public ExteriorDrawer() : base() { }

        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // Get behaviours
            if (target is not ExteriorBehaviour exterior) return;
           
            // Get modules
            var connectMod = exterior.OwnerLayer.GetModule<ConnectedTileMapModule>();
            
            // Paint new tiles
            foreach (LBSTile newTile in exterior.RetrieveNewTiles())
            {
                var connections = connectMod.GetConnections(newTile);
                
                var tView = GetTileView(newTile, connections, teselationSize);
                
                // Stores using LBSTile as key
                view.AddElement(exterior.OwnerLayer, newTile, tView);
            }
            
            // Update stored tiles
            foreach (LBSTile tile in exterior.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElements(exterior.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    var tView = (ExteriorTileView)graphElement;
                    if(tView == null) continue;

                    var connections = connectMod.GetConnections(tile);
                    UpdateTileView(ref tView, tile, connections, teselationSize);
                }
            }
            
            // Paint all tiles
            if (_loaded) return;
            foreach (var tile in exterior.Tiles)
            {   
                var connections = connectMod.GetConnections(tile);
                var tView = GetTileView(tile, connections, teselationSize);
                
                // Stores using LBSTile as key
                view.AddElement(exterior.OwnerLayer, tile, tView);
                exterior.Keys.Add(tile);
            }
            _loaded = true;
        }

        public override void ShowVisuals(object target, MainView view, Vector2 teselationSize)
        {
            Debug.Log("Exterior Drawer is showing");
            // Get behaviours
            if (target is not ExteriorBehaviour exterior) return;
            
            foreach (LBSTile tile in exterior.Keys)
            {
                foreach (var graphElement in view.GetElements(exterior.OwnerLayer, tile).Where(graphElement => graphElement != null))
                {
                    graphElement.visible = true;
                }
            }
        }
        public override void HideVisuals(object target, MainView view, Vector2 teselationSize)
        {
            Debug.Log("Exterior Drawer is hiding");
            // Get behaviours
            if (target is not ExteriorBehaviour exterior) return;
            
            foreach (LBSTile tile in exterior.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElements(exterior.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.visible = false;
                }
            }
        }

        public override void ReDraw(LBSLayer layer, object[] olds, object[] news, MainView view, Vector2 teselationSize)
        {
            Debug.Log("REDRAW bit EXTERIOR");

            // Get modules
            var tileMod = layer.GetModule<TileMapModule>();
            var connectMod = layer.GetModule<ConnectedTileMapModule>();

            var c = Mathf.Max(olds.Length, news.Length);

            for (int i = 0; i < c; i++)
            {
                var o = olds[i];
                var n = news[i];

                if (o != null && n != null)
                {
                    // TODO: add REPLACE action
                }
                else if (o == null && n != null)
                {
                    if (n.GetType().Equals(typeof(LBSTile)))
                    {
                        var tile = n as LBSTile;
                    
                        var connections = connectMod.GetConnections(tile);
                        var ve = GetTileView(tile, connections, teselationSize);
                        view.AddElement(layer, this, ve);
                    }
                }
                else if (o != null && n == null)
                {
                    // TODO: add REMOVE action
                }
            }
        }

        private GraphElement GetTileView(LBSTile tile, List<string> connections, Vector2 teselationSize)
        {
            ExteriorTileView tView = new ExteriorTileView(connections);
            
            //if(tile.tag) tView.SetTileCenter(tile.tag);
            tView.SetConnections(connections.ToArray());
            
            var pos = new Vector2(tile.Position.x, -tile.Position.y);

            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(pos * size, size));

            return tView;
        }

        private void UpdateTileView(ref ExteriorTileView tView, LBSTile tile, List<string> connections, Vector2 teselationSize)
        {
            tView.SetConnections(connections.ToArray());
            var pos = new Vector2(tile.Position.x, -tile.Position.y);

            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(pos * size, size));
        }

        public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
        {
            var exterior = target as ExteriorBehaviour;

            var tileMod = exterior.OwnerLayer.GetModule<TileMapModule>();
            var connectMod = exterior.OwnerLayer.GetModule<ConnectedTileMapModule>();

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

        private Texture2D GetTileTexture(List<string> connections, Vector2Int teselationSize)
        {
            var texture = new Texture2D(teselationSize.x, teselationSize.y);

            for (int j = 0; j < teselationSize.y; j++)
            {
                for (int i = 0; i < teselationSize.x; i++)
                {
                    //UP and DOWN Might Change on MIRROR
                    if (i < teselationSize.x * 0.33) // LEFT
                    {
                        if (j < teselationSize.y * 0.33 || j > teselationSize.y * 0.66)
                            continue;
                        var index = Directions.Bidimencional.Edges.FindIndex(v => v == Vector2.left);
                        var connection = connections[index];
                        var color = Identifiers.Find(t => t.Label.Equals(connection)).Color;
                        texture.SetPixel(i, j, color);
                    }
                    else if (i > teselationSize.x * 0.66) // RIGHT
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

                    if ((i < teselationSize.x * 0.33 || i > teselationSize.x * 0.66) && (j < teselationSize.y * 0.33 || j > teselationSize.y * 0.66))
                    {
                        texture.SetPixel(i, j, Color.gray);
                    }

                }
            }

            return texture;
        }
    }
}