using ISILab.LBS.Assistants;
using LBS.Components;
using ISILab.LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveAreaConnection : LBSManipulator
    {
        HillClimbingAssistant hillclimbing;

        public RemoveAreaConnection() : base()
        {
        }

        public override void Init(LBSLayer layer, object owner)
        {
            hillclimbing = owner as HillClimbingAssistant;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Zone Conection");

            Vector2 pos = position / (hillclimbing.Owner.TileSize * LBSSettings.Instance.general.TileSize);
            pos = new Vector2(pos.x, -(pos.y - 1));

            hillclimbing.RemoveZoneConnection(pos, 0.2f);


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}