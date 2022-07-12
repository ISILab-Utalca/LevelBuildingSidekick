using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LevelBuildingSidekick
{
    public class LBSView : View
    {
        bool creatingNew = true;

        string loadLevelName;

        string newLevelName;
        Vector2Int newLevelSize;
        string step1Key;
        LevelRepresentationData step1;
        //LevelRepresentationData step2;
        //LevelRepresentationData step3;
        //LevelRepresentationData step4;
        //LevelRepresentationData step5;

        System.Action closeWindow;

        List<string> jsonFiles = new List<string>();

        int levelToLoad = 0;
        public LBSView(Controller controller):base(controller)
        {
            //Window = EditorWindow.GetWindow<LBSWindow>();
            jsonFiles = Utility.JSONDataManager.GetJSONFiles(Application.persistentDataPath);
        }
        public override void Draw2D()
        {
        }

        public override void DrawEditor()
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

                    LBSController.Instance.CurrentLevel = LBSController.Instance.CreateLevel(newLevelName, newLevelSize);
                }
                else
                { 
                    LBSController.Instance.SetLevel(Utility.JSONDataManager.LoadData<LevelData>(loadLevelName));
                }
                //Debug.Log("View: " + LBSController.Instance.CurrentLevel.View);
                LBSController.Instance.CurrentStep.View.Display2DWindow();
                LBSController.Instance.CurrentStep.View.DisplayInspectorWindow();
                closeWindow();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        public override void Display2DWindow()
        {
        }

        public override void DisplayInspectorWindow()
        {
            var window = LBSController.Instance.RequestWindow("LBSWindow");
            window.init = () => jsonFiles = Utility.JSONDataManager.GetJSONFiles(Application.persistentDataPath);
            window.onFocus = () => jsonFiles = Utility.JSONDataManager.GetJSONFiles(Application.persistentDataPath);
            window.draw = DrawEditor;
            closeWindow = window.Close;
            window.Show();
        }
    }
}

