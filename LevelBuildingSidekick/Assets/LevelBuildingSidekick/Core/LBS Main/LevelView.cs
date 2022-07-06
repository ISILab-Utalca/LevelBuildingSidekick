using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LevelBuildingSidekick
{
    public class LevelView : View
    {
        public bool openLevel;
        public bool openTags;
        public bool openGameObjects;
        public int targetKey = 0;
        public LevelView(Controller controller): base(controller)
        {

        }

        public override void DrawEditor()
        {
            LevelController controller = Controller as LevelController;

            GUILayout.Label("Level Data", EditorStyles.boldLabel);

            //controller.LevelSize = EditorGUILayout.Vector2IntField("Level Size ", controller.LevelSize);

            #region TAGS
            openTags = EditorGUILayout.BeginFoldoutHeaderGroup(openTags, "Tags");
            //controller.Tags = EditorGUILayout.TextField()
            var list = controller.Tags.ToList();

            /*int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);*/
            int erase = -1;
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list[i] = EditorGUILayout.TextField("Element " + i, list[i]).ToUpperInvariant();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    erase = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            string newTag = "";
            newTag = EditorGUILayout.TextField("Element " + list.Count, newTag).ToUpperInvariant();

            if (erase >= 0 && erase < list.Count)
            {
                list.RemoveAt(erase);
            }

            if(!newTag.Trim(' ','.',',','\n','\t').Equals(""))
            {
                list.Add(newTag);
            }

            controller.Tags = list.Distinct().ToHashSet();
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region PREFABS
            openGameObjects = EditorGUILayout.BeginFoldoutHeaderGroup(openGameObjects, "Prefabs");

            var keys = controller.LevelObjects.Select((k => k.Key)).ToList();
            keys.Add("New Category");

            targetKey = EditorGUILayout.Popup(targetKey, keys.ToArray());

            string key = keys[targetKey];
            string newKey = key;

            if (targetKey == controller.LevelObjects.Count())
            {
                controller.LevelObjects.Add("New Category", new HashSet<GameObject>());
            }

            //Debug.Log(targetKey);
            newKey = EditorGUILayout.TextField("Prefab Type: ", newKey);

            var prefabs = controller.LevelObjects[keys[targetKey]].ToList();

            if(!key.Equals("New Category"))
            {
                erase = -1;
                for (int i = 0; i < prefabs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    prefabs[i] = EditorGUILayout.ObjectField("Element " + i, prefabs[i], typeof(GameObject), false) as GameObject;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-"))
                    {
                        erase = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GameObject newPref = null;
                newPref = EditorGUILayout.ObjectField("Element " + prefabs.Count, newPref, typeof(GameObject), false) as GameObject;

                if (erase >= 0 && erase < prefabs.Count)
                {
                    prefabs.RemoveAt(erase);
                }

                if (newPref != null)
                {
                    prefabs.Add(newPref);
                }
            }
            

            if(!key.Equals(newKey))
            {
                if(!controller.LevelObjects.ContainsKey(newKey))
                {
                    controller.LevelObjects.Remove(key);
                    controller.LevelObjects.Add(newKey, prefabs.Distinct().ToHashSet());
                }
            }
            else
            {
                controller.LevelObjects[key] = prefabs.Distinct().ToHashSet();
            }


            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion
        }
    }
}


