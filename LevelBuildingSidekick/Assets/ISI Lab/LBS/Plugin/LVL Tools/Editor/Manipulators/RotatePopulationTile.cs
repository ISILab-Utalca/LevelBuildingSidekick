using ISILab.Commons;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RotatePopulationTile : LBSManipulator
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        PopulationBehaviour population;
        private Vector2Int first;

        public RotatePopulationTile()
        {
            feedback = new ConnectedLine();
            feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object provider)
        {
            population = provider as PopulationBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
        {
            first = population.Owner.ToFixedPosition(startPosition);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Rotate");

            var pos = population.Owner.ToFixedPosition(endPosition);

            var dx = first.x - pos.x;
            var dy = first.y - pos.y;

            var fDir = Directions.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

            if (fDir < 0 || fDir >= Directions.Count)
                return;

            population.RotateTile(first, Directions[fDir]);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}