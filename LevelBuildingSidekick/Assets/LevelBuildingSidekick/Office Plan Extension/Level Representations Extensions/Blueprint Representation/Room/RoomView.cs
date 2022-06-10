using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick.Blueprint
{
    public class RoomView : View
    {
        public RoomView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {

        }

        public override void DrawEditor()
        {
            RoomController roomController = Controller as RoomController;

            roomController.Label = EditorGUILayout.TextField("Label", roomController.Label);
            
            roomController.ProportionType = (ProportionType)EditorGUILayout.EnumPopup("Proportion type", roomController.ProportionType);
            switch (roomController.ProportionType)
            {
                case ProportionType.RATIO:
                    roomController.Ratio = EditorGUILayout.Vector2IntField("Aspect Radio ", roomController.Ratio);
                    break;
                case ProportionType.SIZE:
                    roomController.Width = EditorGUILayout.Vector2IntField("Width ", roomController.Width);
                    roomController.Height = EditorGUILayout.Vector2IntField("Height", roomController.Height);
                    break;
            }

        }
    }
}

