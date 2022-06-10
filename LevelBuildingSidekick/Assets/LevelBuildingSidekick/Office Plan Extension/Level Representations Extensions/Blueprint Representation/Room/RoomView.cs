using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick.Blueprint
{
    public class RoomView : View
    {
        bool ftDropdown;
        bool wtDropdown;
        bool dtDropdown;

        public RoomView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {

        }

        public override void DrawEditor()
        {
            RoomController controller = Controller as RoomController;

            controller.Label = EditorGUILayout.TextField("Label", controller.Label);
            
            controller.ProportionType = (ProportionType)EditorGUILayout.EnumPopup("Proportion type", controller.ProportionType);
            switch (controller.ProportionType)
            {
                case ProportionType.RATIO:
                    controller.Ratio = EditorGUILayout.Vector2IntField("Aspect Radio ", controller.Ratio);
                    break;
                case ProportionType.SIZE:
                    controller.Width = EditorGUILayout.Vector2IntField("Width ", controller.Width);
                    controller.Height = EditorGUILayout.Vector2IntField("Height", controller.Height);
                    break;
            }

            EditorGUILayout.Space();

            ftDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(ftDropdown, "Floor Tiles");
            if (ftDropdown)
            {
                var list = controller.FloorTiles;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = EditorGUILayout.ObjectField("Element " + i, list[i], typeof(GameObject), true) as GameObject;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();

            wtDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(wtDropdown, "Wall Tiles");
            if (wtDropdown)
            {
                var list = controller.WallTiles;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = EditorGUILayout.ObjectField("Element " + i, list[i], typeof(GameObject), true) as GameObject;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();

            dtDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(dtDropdown, "Door Tiles");
            if (dtDropdown)
            {
                var list = controller.DoorTiles;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = EditorGUILayout.ObjectField("Element " + i, list[i], typeof(GameObject), true) as GameObject;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}

