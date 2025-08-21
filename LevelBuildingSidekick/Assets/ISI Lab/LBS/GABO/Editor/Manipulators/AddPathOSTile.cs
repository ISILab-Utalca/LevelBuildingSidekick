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

        protected override string IconGuid => null;
        #endregion

        public AddPathOSTile() : base()
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);

            behaviour = provider as PathOSBehaviour;
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
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
                    behaviour.AddTile(ToSet.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0].Value.ToLBSTag(), i, j);
                    //behaviour.AddTile(ToSet.GetCharacteristics<LBSTagsCharacteristic>()[0].Value, i, j);
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
