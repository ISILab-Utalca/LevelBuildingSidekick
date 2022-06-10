using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick.Graph
{
    public class NodeView : View
    {
        public Texture2D circle;
        Vector2 scrollPos;
        bool ftDropdown;
        bool wtDropdown;
        bool dtDropdown;

        public NodeView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {

            //Debug.Log("Node View");
            var node = Controller as NodeController;

            var pos = node.Position;
            var size = 2 * node.Radius * Vector2.one;

            Rect rect = new Rect(pos, size);
            Rect innerRect = new Rect(pos + (size * 0.2f), size * 0.6f); //0.7 == sqrt(2)/2, side of square inside circle inside square.
                                                                         //should be 0,15 but image has blank space

            GUI.DrawTexture(rect, node.Sprite, ScaleMode.StretchToFill);

            GUILayout.BeginArea(innerRect);
            GUILayout.Label(node.Label);
            //scrollPos = GUILayout.BeginScrollView(scrollPos);
            //GUILayout.Button(data.Sprite);
            //Rect rt = GUILayoutUtility.GetAspectRect(1);
            //rt.position = Vector2.zero;
            //rt.size = Vector2.one * 2 * data.Radius;
            //GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override void DrawEditor()
        {
             NodeController controller = Controller as NodeController;
            //Espacio para proximo control
            EditorGUILayout.Space();
            
            controller.Label = EditorGUILayout.TextField("Label ", controller.Label);
           
            //Espacio para proximo control
            EditorGUILayout.Space();
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

            //Espacio para proximo control
            EditorGUILayout.Space();

            ftDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(ftDropdown, "Floor Tiles");
            if(ftDropdown)
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


