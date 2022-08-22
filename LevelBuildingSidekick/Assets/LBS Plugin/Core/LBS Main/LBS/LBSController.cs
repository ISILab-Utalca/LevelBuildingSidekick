using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using LevelBuildingSidekick.Graph;

namespace LevelBuildingSidekick
{
    public class LBSController /// change name "LBSController" to "LBS" or "LBSCore"
    {
        #region InspectorDrawer
        private class LevelScriptable : GenericScriptable<LevelData> { };
        [CustomEditor(typeof(LevelScriptable)),CanEditMultipleObjects]
        private class LevelScriptableEditor : GenericScriptableEditor { };
        #endregion

        private static string currentPath;
        private static LevelBackUp backUp;

        public static LevelData CurrentLevel
        {
            get
            {
                LoadBackup();
                return backUp.level;
            }
            set
            {
                LoadBackup();
                backUp.level = value;
            }
        }


        private static void LoadBackup()
        {
            if (backUp == null)
            {
                backUp = Resources.Load("LBSBackUp") as LevelBackUp;
                if (backUp == null)
                {
                    backUp = ScriptableObject.CreateInstance<LevelBackUp>();
                    if(!Directory.Exists("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources")) // esto podria ser peligroso (!)
                    {
                        Directory.CreateDirectory("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources"); // esto podria ser peligroso (!)
                    }
                    AssetDatabase.CreateAsset(backUp, "Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources/LBSBackUp.asset"); // esto podria ser peligroso (!)
                    AssetDatabase.SaveAssets();
                }
            }
        }


        internal static void SaveFile()
        {
            if(CurrentLevel.levelName == "")
                SaveFileAs();

            var path = Application.dataPath;
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            var files = Utility.JSONDataManager.GetAllFilesByExtencion(".json", dir);

            // esto no me deja tener dos archivos que se llamen igual en carpetas diferentes
            var fileInfo = files.Find(f => f.Name.Contains(CurrentLevel.levelName));

            if (fileInfo != null)
            {
                Utility.JSONDataManager.SaveData(fileInfo.FullName, LBSController.CurrentLevel);
            }
            else
            {
                SaveFileAs();
            }

        }

        internal static void SaveFileAs()
        {
            var lvl = CurrentLevel;

            var name = lvl.levelName;
            var path = EditorUtility.SaveFilePanel("Save level data", "", name + ".json", "json");

            if (path != "")
            {
                Debug.Log("Save file on: '" + path + "'.");
                Utility.JSONDataManager.SaveData(path, LBSController.CurrentLevel);
            }
        }

        public static LevelData CreateLevel(string levelName, Vector3 size)
        {
            LevelData data = new LevelData();
            data.levelName = levelName;
            data.Size = size;
            data.representations.Add(new LBSGraphData());
            return data;
        }

        public static void ShowLevelInspector()
        {
            var s = ScriptableObject.CreateInstance<LevelScriptable>();
            s.data = CurrentLevel;
            Selection.SetActiveObjectWithContext(s, s);
        }
    }
}


