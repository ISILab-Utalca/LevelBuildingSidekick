using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveTile : ManipulateTeselation
    {
        protected override string IconGuid { get => "ce08b36a396edbf4394f7a4e641f253d"; }

        public RemoveTile():base(){}
        
        public override void Init(LBSLayer layer, object behaviour)
        {
            base.Init(layer, behaviour);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var min = module.OwnerLayer.ToFixedPosition(Vector2Int.Min(StartPosition, EndPosition));
            var max = module.OwnerLayer.ToFixedPosition(Vector2Int.Max(StartPosition, EndPosition));

            for (int i = min.x; i <= max.x; i++)
            {
                for (int j = min.y; j <= max.y; j++)
                {
                    var pos = new Vector2Int(i, j);

                    var tile = module.GetTile(pos);

                    if (tile == null)
                        continue;

                    module.RemoveTile(tile);
                }
            }
        }
    }
}