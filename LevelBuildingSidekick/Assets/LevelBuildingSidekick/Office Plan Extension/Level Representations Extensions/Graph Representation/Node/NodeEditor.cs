using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditorInternal;

namespace LevelBuildingSidekick.Graph
{
    [CustomEditor(typeof(NodeData))]
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
            //base.OnInspectorGUI();
            NodeData g = target as NodeData;
            //Espacio para proximo control
            EditorGUILayout.Space();
            
            g.label = EditorGUILayout.TextField("Label ", g.label);
           
            //Espacio para proximo control
            EditorGUILayout.Space();
            g.proportionType = (ProportionType)EditorGUILayout.EnumPopup("Proportion type", g.proportionType);

            switch (g.proportionType)
            {
                case ProportionType.RATIO:
                    g.aspectRatio = EditorGUILayout.Vector2IntField("Aspect Radio ", g.aspectRatio);
                    break;
                case ProportionType.SIZE:
                    g.width = EditorGUILayout.Vector2IntField("Width ", g.width);
                    g.height = EditorGUILayout.Vector2IntField("Height", g.height);
                    break;
            }

            //Espacio para proximo control
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            var listFloor = g.floorTiles;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", listFloor.Count));
            
            
            while (newCount < listFloor.Count)
                listFloor.RemoveAt(listFloor.Count - 1);
            while (newCount > listFloor.Count)
                listFloor.Add(null);

            for (int i = 0; i < listFloor.Count; i++)
            {
                //listFloor[i] = EditorGUILayout.TextField("Element: " + i, listFloor[i]);
            }
            EditorGUILayout.EndHorizontal();

        }

       
        
    }
}

