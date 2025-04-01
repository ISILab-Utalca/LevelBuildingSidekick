using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveSchemaTile : LBSManipulator
    {
        SchemaBehaviour schema;

        public RemoveSchemaTile() : base()
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            schema = owner as SchemaBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Zone");

            var corners = schema.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    schema.RemoveTile(new Vector2Int(i, j));
                }
            }

            schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}