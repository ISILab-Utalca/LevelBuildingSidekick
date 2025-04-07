using ISILab.LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveTileExterior : ManipulateTeselation
    {
        private ExteriorBehaviour exterior;

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);

            exterior = owner as ExteriorBehaviour;
            lbsLayer = layer;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Tiles");

            var corners = exterior.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tile = exterior.GetTile(pos);
                    if (tile == null)
                        continue;
                    exterior.RemoveTile(tile);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}