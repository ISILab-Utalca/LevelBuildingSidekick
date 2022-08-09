using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LevelBuildingSidekick
{
    public class LBSView : GenericWindow
    {
        bool creatingNew = true;

        string loadLevelName;

        string newLevelName;
        Vector2Int newLevelSize;
        string step1Key;
        LevelRepresentationData step1;
        List<string> jsonFiles = new List<string>();
        int levelToLoad = 0;

        private void Awake()
        {
            init = () => jsonFiles = Utility.JSONDataManager.GetJSONFiles(Application.dataPath + "/LBSLevels");
            onFocus = () => jsonFiles = Utility.JSONDataManager.GetJSONFiles(Application.dataPath + "/LBSLevels");
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("New Level", GUILayout.MaxHeight(128)))
            {
                creatingNew = true;
            }
            if (GUILayout.Button("Load Level", GUILayout.MaxHeight(128)))
            {
                creatingNew = false;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (creatingNew)
            {
                newLevelName = EditorGUILayout.TextField("Level Name: ", newLevelName);
                EditorGUILayout.Space();
                newLevelSize = EditorGUILayout.Vector2IntField("level Size: ", newLevelSize);
                /*var keys = LBSController.Instance.LevelRepresentations.Select((p) => p.Key).ToArray();
                int step1index = 0;
                if(LBSController.Instance.LevelRepresentations.ContainsKey(step1Key))
                {

                }

                step1index = EditorGUILayout.Popup("Step1 ", step1index, keys);*/
            }
            else
            {
                levelToLoad = EditorGUILayout.Popup("Level to load: ", levelToLoad, jsonFiles.ToArray());
                if(jsonFiles.Count > 0)
                {
                    loadLevelName = jsonFiles[levelToLoad];
                }
            }

            if (GUILayout.Button("Open", GUILayout.MaxHeight(128)))
            {
                if (creatingNew)
                {

                    LBSController.CurrentLevel = LBSController.CreateLevel(newLevelName, newLevelSize);
                }
                else
                { 
                    LBSController.SetLevel(Utility.JSONDataManager.LoadData<LevelData>("LBSLevels", loadLevelName));
                }
                //Debug.Log("View: " + LBSController.Instance.CurrentLevel.View);
                LBSController.CurrentLevel.CurrentRepresentation.View.Display2DWindow();
                LBSController.CurrentLevel.CurrentRepresentation.View.DisplayInspectorWindow();
                Close();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
}

