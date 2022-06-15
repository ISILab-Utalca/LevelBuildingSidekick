using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LevelBuildingSidekick
{
    public class LevelView : View
    {
        public LevelView(Controller controller): base(controller)
        {

        }

        public override void DrawEditor()
        {
            LevelController controller = Controller as LevelController;

            controller.LevelSize = EditorGUILayout.Vector2IntField("Level Size ", controller.LevelSize);

            //controller.Tags = EditorGUILayout.TextField()
            var list = controller.Tags.ToList();
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = EditorGUILayout.TextField("Element " + i, list[i]);
            }

            controller.Tags = list.ToHashSet();
        }
    }
}


