using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
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
            get => schema.roomToSet;
            set => schema.roomToSet = value;
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
            //Undo.RecordObjects(schema., "transform selected objects");

            if (e.ctrlKey)
            {
                var newZone = schema.AddZone();
                newZone.InsideStyles = new List<string>() { schema.PressetInsideStyle.Name };
                newZone.OutsideStyles = new List<string>() { schema.PressetOutsideStyle.Name };

                ToSet = newZone;
            }

            if (ToSet == null)
            {
                Debug.LogWarning("No tienens ninguna zona seleccionada para colocar.");
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
        }
    }
}