using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditorInternal;

namespace LevelBuildingSidekick.Graph
{
    [CustomEditor(typeof(NodeController))]
    public class NodeEditor : Editor
    {

        // Start is called before the first frame update
        private void OnEnable()
        {
            
        }

        // Update is called once per frame
        public override void OnInspectorGUI()
        {

            //base.OnInspectorGUI();
            NodeController nodeController = 
            //Espacio para proximo control
            EditorGUILayout.Space();
            
            nodeController.Label = EditorGUILayout.TextField("Label ", nodeController.Label);
           
            //Espacio para proximo control
            EditorGUILayout.Space();
            nodeController.ProportionType = (ProportionType)EditorGUILayout.EnumPopup("Proportion type", nodeController.ProportionType);

            switch (nodeController.ProportionType)
            {
                case ProportionType.RATIO:
                    nodeController.Ratio = EditorGUILayout.Vector2IntField("Aspect Radio ", nodeController.Ratio);
                    break;
                case ProportionType.SIZE:
                    nodeController.Width = EditorGUILayout.Vector2IntField("Width ", nodeController.Width);
                    nodeController.Height = EditorGUILayout.Vector2IntField("Height", nodeController.Height);
                    break;
            }

            //Espacio para proximo control
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            

            /*
            var listFloor = NodeController.floorTiles;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", listFloor.Count));
            
            
            while (newCount < listFloor.Count)
                listFloor.RemoveAt(listFloor.Count - 1);
            while (newCount > listFloor.Count)
                listFloor.Add(null);

            for (int i = 0; i < listFloor.Count; i++)
            {
                //listFloor[i] = EditorGUILayout.TextField("Element: " + i, listFloor[i]);
            }
            */
            EditorGUILayout.EndHorizontal();

        }

       
        
    }
}

