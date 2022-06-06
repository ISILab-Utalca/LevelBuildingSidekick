using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditorInternal;

namespace LevelBuildingSidekick.Graph
{
    public class NodeEditor : Editor
    {
        SerializedProperty node;
        ReorderableList floorTiles;
        ReorderableList wallTiles;
        ReorderableList doorTiles;

        // Start is called before the first frame update
        private void OnEnable()
        {
            node = serializedObject.FindProperty("node");
            NodeData d = target as NodeData;
            if (d.floorTiles == null)
            {
                d.floorTiles = new List<GameObject>();
            }
            if (d.wallTiles == null)
            {
                d.wallTiles = new List<GameObject>();
            }
            if (d.doorTiles == null)
            {
                d.doorTiles = new List<GameObject>();
            }
        }

        // Update is called once per frame
        public override void OnInspectorGUI()
        {
            NodeData d = target as NodeData;
            d.name = EditorGUILayout.TextField("Name: ", d.name);
            d.width = EditorGUILayout.Vector2IntField("Width: ", d.width);
            d.height = EditorGUILayout.Vector2IntField("Height: ", d.height);
            d.aspectRatio = EditorGUILayout.Vector2IntField("Aspect Ratio: ", d.aspectRatio);

            floorTiles = new ReorderableList(d.floorTiles, typeof(List<GameObject>), true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    GUI.Label(rect, "Floor Tiles");
                },
                drawElementCallback = (rect, index, active, focused) =>
                {
                }

            };
            floorTiles.DoLayoutList();

            wallTiles = new ReorderableList(d.wallTiles, typeof(List<GameObject>), true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    GUI.Label(rect, "Wall Tiles");
                },
                drawElementCallback = (rect, index, active, focused) =>
                {
                    Type t = d.wallTiles.GetType();
                    SerializedProperty element = serializedObject.FindProperty("wallTiles").GetArrayElementAtIndex(index);
                    EditorGUI.ObjectField(rect, element, t, new GUIContent("Element " + index));
                    d.wallTiles[index] = element.objectReferenceValue as GameObject;
                },
            };
            wallTiles.DoLayoutList();

            doorTiles = new ReorderableList(d.doorTiles, typeof(List<GameObject>), true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    GUI.Label(rect, "Door Tiles");
                },
                drawElementCallback = (rect, index, active, focused) =>
                {
                    Type t = d.doorTiles.GetType();
                    SerializedProperty element = serializedObject.FindProperty("doorTiles").GetArrayElementAtIndex(index);
                    EditorGUI.ObjectField(rect, element, t, new GUIContent("Element " + index));
                    d.doorTiles[index] = element.objectReferenceValue as GameObject;
                },
            };
            doorTiles.DoLayoutList();

            //base.OnInspectorGUI();
        }
    }
}

