using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveSchemaTile : LBSManipulator
    {
        private SchemaBehaviour _schema;
        protected override string IconGuid => "ce08b36a396edbf4394f7a4e641f253d";
        
        public RemoveSchemaTile()
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;

            Name = "Remove Tile";
            Description = "Select an area to remove any tiles that belong to the selected zone.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _schema = provider as SchemaBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            base.OnMouseUp(element, endPosition, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Zone");

            var corners = _schema.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    _schema.RemoveTile(new Vector2Int(i, j));
                }
            }

            _schema.RecalculateWalls();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}