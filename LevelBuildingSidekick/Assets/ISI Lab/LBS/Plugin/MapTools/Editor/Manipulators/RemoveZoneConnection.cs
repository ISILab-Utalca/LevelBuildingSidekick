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
    public class RemoveZoneConnection : LBSManipulator
    {
        HillClimbingAssistant hillclimbing;

        public RemoveZoneConnection() : base()
        {
            name = "Remove Assistant zone connection";
            description = "Click a connection between zones to remove it.";
        }

        protected override string IconGuid { get => "42830f36abd22544fb35c697171f8374"; }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            hillclimbing = owner as HillClimbingAssistant;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Zone Conection");

            Vector2 pos = position / (hillclimbing.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize);
            pos = new Vector2(pos.x, -(pos.y - 1));

            hillclimbing.RemoveZoneConnection(pos, 0.2f);


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}