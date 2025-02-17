using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using LBS.Components;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddSchemaTile : LBSManipulator
    {
        private SchemaBehaviour schema;

        public Zone ToSet
        {
            get => schema.RoomToSet;
            set => schema.RoomToSet = value;
        }

        public AddSchemaTile() : base()
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object owner)
        {
            schema = owner as SchemaBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(x, "Add Zone");
            EditorGUI.BeginChangeCheck();

            if (e.ctrlKey)
            {
                var newZone = schema.AddZone();
                newZone.InsideStyles = new List<string>() { schema.PressetInsideStyle.Name };
                newZone.OutsideStyles = new List<string>() { schema.PressetOutsideStyle.Name };

                ToSet = newZone;
            }

            if(!schema.Zones.Contains(ToSet)) { ToSet = null; }

            if (ToSet == null)
            {
                Debug.LogWarning("You don't have any selected area to place.");
                return;
            }


            var corners = schema.Owner.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var tile = schema.AddTile(new Vector2Int(i, j), ToSet);
                    schema.AddConnections(
                        tile,
                        new List<string>() { "", "", "", "" },
                        new List<bool> { true, true, true, true }
                        );
                }
            }

            LBSInspectorPanel.Instance.SetTarget(schema.Owner);

            schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}