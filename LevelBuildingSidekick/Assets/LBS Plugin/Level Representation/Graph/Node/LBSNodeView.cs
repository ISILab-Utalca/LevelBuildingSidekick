using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Reflection;

namespace LevelBuildingSidekick.Graph
{
    public class LBSNodeView : GraphElement
    {
        public Texture2D circle;
        Vector2 scrollPos;
        bool openTags;
        bool openGameObjects;
        int categoryIndex;

        public LBSNodeData Data;

        public delegate void NodeEvent(LBSNodeData data);
        public NodeEvent OnStartDragEdge;
        public NodeEvent OnEndDragEdge;

        public LBSNodeView(LBSNodeData node)
        {
            Data = node;

            SetPosition(new Rect(Data.Position - Vector2.one * Data.Radius, Vector2.one * 2 * Data.Radius));

            Box b = new Box();
            b.style.minHeight = b.style.minWidth = b.style.maxHeight = b.style.maxWidth = 2 * Data.Radius;
            b.Add(new Label(node.label));
            
            Add(b);
            //Add(new Label(Controller.Label));

            VisualElement main = this;
            //VisualElement borderContainer = main.Q(name: "node-border");

            capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("NodeUSS");
            styleSheets.Add(styleSheet);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            //Debug.Log(Controller.Label + " AH!");
            //base.OnSelected();
            Data.UnitySelect();
            //Debug.Log("Calling");
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = new Vector2Int((int)newPos.x, (int)newPos.y);
        }


        

        /*
        public void DrawEditor()
        {
             LBSNodeController controller = Controller as LBSNodeController;
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

            var level = LBSController.CurrentLevel;

            GUILayout.Label("Level Data", EditorStyles.boldLabel);

            //controller.LevelSize = EditorGUILayout.Vector2IntField("Level Size ", controller.LevelSize);

            #region TAGS
            openTags = EditorGUILayout.BeginFoldoutHeaderGroup(openTags, "Tags");
            //controller.Tags = EditorGUILayout.TextField()
            var tags = level.tags.ToList();
            tags.Add("None");
            var myTags = controller.Tags.ToList();

            int erase = -1;
            for (int i = 0; i < myTags.Count; i++)
            {
                int index = -1;
                if (level.tags.Contains(myTags[i]))
                {
                    index = tags.FindIndex((s) => s.Equals(myTags[i]));
                }
                else
                {
                    level.tags.Remove(myTags[i]);
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

            var prefs = LBSController.CurrentLevel.RequestLevelObjects(category).ToList();
            var options = prefs.Select((p) => p.name).ToList();
            var myPrefs = controller.GetPrefabs(category).ToList();

            for (int i = 0; i < myPrefs.Count; i++)
            {
                int index = -1;
                if(LBSController.CurrentLevel.RequestLevelObjects(category).Contains(myPrefs[i]))
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
                LBSController.CurrentLevel.RequestLevelObjects(category).Add(newPref);
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
        */

    }
}


