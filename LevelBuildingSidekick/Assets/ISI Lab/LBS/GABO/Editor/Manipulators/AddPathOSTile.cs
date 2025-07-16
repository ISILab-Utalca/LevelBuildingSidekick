using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
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

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            if (ToSet == null)
            {
                Debug.LogWarning("[ISILab]: You don't have any selected item to place.");
                return;
            }

            //GABO TODO: ARREGLAR LOGICA UNDO
            // Inicio logica UNDO
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add PathOS Tile");

            // Agregar PathOSTiles mediante PathOSBehaviour
            var corners = behaviour.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);
            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    behaviour.AddTile(ToSet.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0].Value, i, j);
                }
            }

            // Final logica UNDO
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }


        }
    }
}
