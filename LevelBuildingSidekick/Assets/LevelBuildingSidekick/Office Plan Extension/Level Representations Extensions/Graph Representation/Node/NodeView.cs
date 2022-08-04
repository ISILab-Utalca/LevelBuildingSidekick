using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LevelBuildingSidekick.Graph
{
    public class NodeView : View
    {
        public Texture2D circle;
        Vector2 scrollPos;
        bool ftDropdown;
        bool wtDropdown;
        bool dtDropdown;
        bool openTags;
        bool openGameObjects;
        int categoryIndex;

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
            string newLabel = controller.Label;
            newLabel = EditorGUILayout.TextField("Label ", newLabel);
            controller.Label = newLabel;
           
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
                    controller.HeightRange = EditorGUILayout.Vector2IntField("Height", controller.HeightRange);
                    break;
            }

            //Espacio para proximo control
            EditorGUILayout.Space();

            LevelController level = LBSController.Instance.CurrentLevel;

            GUILayout.Label("Level Data", EditorStyles.boldLabel);

            //controller.LevelSize = EditorGUILayout.Vector2IntField("Level Size ", controller.LevelSize);

            #region TAGS
            openTags = EditorGUILayout.BeginFoldoutHeaderGroup(openTags, "Tags");
            //controller.Tags = EditorGUILayout.TextField()
            var tags = level.Tags.ToList();
            tags.Add("None");
            var myTags = controller.Tags.ToList();

            /*int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);*/
            int erase = -1;
            for (int i = 0; i < myTags.Count; i++)
            {
                int index = -1;
                if (level.Tags.Contains(myTags[i]))
                {
                    index = tags.FindIndex((s) => s.Equals(myTags[i]));
                }
                else
                {
                    level.Tags.Remove(myTags[i]);
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                int newIndex = index;
                newIndex = EditorGUILayout.Popup(newIndex, tags.ToArray());
                if(newIndex != index)
                {
                    myTags[i] = tags[newIndex];
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    erase = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            int newTag = tags.Count - 1;
            newTag = EditorGUILayout.Popup(newTag, tags.ToArray());

            if (erase >= 0 && erase < myTags.Count)
            {
                myTags.RemoveAt(erase);
            }

            if (newTag != tags.Count - 1)
            {
                myTags.Add(tags[newTag]);
            }

            controller.Tags = myTags.Distinct().ToHashSet();
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region PREFABS
            openGameObjects = EditorGUILayout.BeginFoldoutHeaderGroup(openGameObjects, "Prefabs");

            categoryIndex = EditorGUILayout.Popup(categoryIndex, controller.ItemCategories);
            string category = controller.ItemCategories[categoryIndex];

            var prefs = LBSController.Instance.CurrentLevel.RequestLevelObjects(category).ToList();
            var options = prefs.Select((p) => p.name).ToList();
            var myPrefs = controller.GetPrefabs(category).ToList();

            for (int i = 0; i < myPrefs.Count; i++)
            {
                int index = -1;
                if(LBSController.Instance.CurrentLevel.RequestLevelObjects(category).Contains(myPrefs[i]))
                {
                    index = prefs.FindIndex((p) => p.Equals(myPrefs[i]));
                }
                else
                {
                    myPrefs.Remove(myPrefs[i]);
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                index = EditorGUILayout.Popup(index, options.ToArray());
                myPrefs[i] = prefs[index];
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    erase = i;
                }
                EditorGUILayout.EndHorizontal();
            }

            options.Add("New Prefab");
            int newDDPref = options.Count - 1;
            newDDPref = EditorGUILayout.Popup(newDDPref, options.ToArray());
            GameObject newPref = null;
            newPref = EditorGUILayout.ObjectField("Element " + myPrefs.Count, newPref, typeof(GameObject), false) as GameObject;

            if (erase >= 0 && erase < myPrefs.Count)
            {
                myPrefs.RemoveAt(erase);
            }

            if (newPref != null)
            {
                LBSController.Instance.CurrentLevel.RequestLevelObjects(category).Add(newPref);
                myPrefs.Add(newPref);
            }

            if (newDDPref < prefs.Count)
            {
                myPrefs.Add(prefs[newDDPref]);
            }

            controller.SetPrefabs(category, myPrefs.ToHashSet());
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

        }
    }
}


