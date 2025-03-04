using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Bundles;
using LBS.Components;
using ISILab.LBS.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddPopulationTile : LBSManipulator
    {
        PopulationBehaviour population;

        public Bundle ToSet
        {
            get => population.selectedToSet;
        }

        public AddPopulationTile() : base()
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object owner)
        {
            population = owner as PopulationBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            if (e.ctrlKey)
            {
                var bundleTile = population.GetTileGroup(population.Owner.ToFixedPosition(endPosition));
                var bundleName = bundleTile?.BundleData.BundleName;
                LBSMainWindow.MessageNotify("Highlighted Element " + population.Owner.ToFixedPosition(endPosition).ToString() + " : " + (bundleName!=null ? bundleName + "\nB: " + bundleTile.GetBounds().ToString() + "/ C: " + bundleTile.GetCenter().ToString() : "None "));
                return;
            }



            if (ToSet == null)
            {
                LBSMainWindow.MessageNotify("You don't have any selected item to place.", LogType.Error);
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Element population");

            var corners = population.Owner.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    population.AddTileGroup(new Vector2Int(i, j), ToSet);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}