using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemovePopulationTile : LBSManipulator
    {
        private PopulationBehaviour _population;
        protected override string IconGuid => "ce08b36a396edbf4394f7a4e641f253d";

        public RemovePopulationTile()
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
            Name = "Remove Tile";
            Description = "Click on an item in the graph to remove it.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _population = provider as PopulationBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove element population");

            var corners = _population.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    Vector2Int position = new Vector2Int(i, j);
                    _population.RemoveTileGroup(position);
                }
            }


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
            
        }
    }
}