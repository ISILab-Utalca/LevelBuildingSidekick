using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    // TODO CHECK IF THIS CLASS CAN BE DELETED. SEEMS TO BE DEPRECATED
    public class AddConnection : LBSManipulator
    {
        private readonly LBSTag _tagToSet;
        private ConnectedTileMapModule _module;

        private static List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        protected override string IconGuid => null;

        private TileConnectionsPair _first;

        public AddConnection(LBSTag tagToSet)
        {
            _tagToSet = tagToSet;
            Feedback = new ConnectedLine();
            Feedback.fixToTeselation = true;
            
            Name = "Add Connection";
            Description = "Add a connection to the selected area.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _module = layer.GetModule<ConnectedTileMapModule>();
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int position, MouseDownEvent e)
        {
            var pos = _module.OwnerLayer.ToFixedPosition(position);
            var tile = _module.Pairs.Find(t => t.Tile.Position == pos);

            if (tile == null)
                return;

            _first = tile;
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            if (_first == null)
                return;

            var pos = _module.OwnerLayer.ToFixedPosition(position);

            var dx = _first.Tile.Position.x - pos.x;
            var dy = _first.Tile.Position.y - pos.y;
            var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

            if (fDir < 0 || fDir >= Directions.Count)
                return;

            var tile = _module.Pairs.Find(t => t.Tile.Position == pos);

            if (tile == null)
            {
                _first.SetConnection(fDir, _tagToSet.Label, false);
                return;
            }

            if (Equals(_first, tile))
                return;

            if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
                return;

            var tDir = Directions.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

            _first.SetConnection(fDir, _tagToSet.Label, false);
            tile.SetConnection(tDir, _tagToSet.Label, false);
        }
    }
}