using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Bundles;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

namespace ISILab.LBS.Manipulators
{
    public class AddPathOSTile : LBSManipulator
    {
        #region FIELDS
        PathOSBehaviour behaviour;
        #endregion

        #region PROPERTIES
        public Bundle ToSet
        {
            get => behaviour.selectedToSet;
        }
        #endregion

        public AddPathOSTile() : base()
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object owner)
        {
            behaviour = owner as PathOSBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        //GABO TODO: TERMINAR
        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            if (ToSet == null)
            {
                Debug.LogWarning("[ISILab]: You don't have any selected item to place.");
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add PathOS Tile");

            var corners = behaviour.Owner.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    // GABO TODO TERMINARR: hacer funcion addtile para pathosbehaviour
                    //population.AddTile(new Vector2Int(i, j), ToSet);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }


        }
    }
}
