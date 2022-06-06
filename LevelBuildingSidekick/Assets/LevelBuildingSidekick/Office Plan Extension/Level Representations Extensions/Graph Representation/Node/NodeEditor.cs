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
           base.OnInspectorGUI();
        }
    }
}

