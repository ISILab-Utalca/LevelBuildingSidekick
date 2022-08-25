using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using LevelBuildingSidekick.Graph;
using System.Text;

namespace LevelBuildingSidekick
{
    public class LBSController /// change name "LBSController" to "LBS" or "LBSCore" or "LBSMain"
    {
        #region InspectorDrawer
        private class LevelScriptable : GenericScriptable<LevelData> { };
        [CustomEditor(typeof(LevelScriptable)),CanEditMultipleObjects]
        private class LevelScriptableEditor : GenericScriptableEditor { };
        #endregion

        private static LevelBackUp backUp;

        public static LevelData CurrentLevel
        {
            get
            {
                var instance = LevelBackUp.Instance();
                return instance.level;
                //LoadBackup();
                //return backUp.level;
            }
            set
            {
                var instance = LevelBackUp.Instance();
                instance.level = value;
                //LoadBackup();
                //backUp.level = value;
            }
        }


        public static List<Type> GetSubClassTypes<T>()
        {
            var result = new List<Type>();
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            Type editorWindowType = typeof(T);
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(editorWindowType))
                    {
                        result.Add(type);
                    }
                }
            }
            return result;
        }

        /*
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
        */

        internal static void LoadFile()
        {
            var answer = EditorUtility.DisplayDialog(
                   "The current file has not been saved",
                   "if you open a file the progress in the current document will be lost, are you sure to continue?",
                   "continue",
                   "cancel");
            if (answer)
            {
                var path = EditorUtility.OpenFilePanel("Load level data", "", ".json");
                CurrentLevel = Utility.JSONDataManager.LoadData<LevelData>(path);
            }
        }

        private static bool FileExists(string name, string extension, out FileInfo toReturn)
        {
            var path = Application.dataPath;
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            var files = Utility.DirectoryTools.GetAllFilesByExtension(extension, dir);

            var fileInfo = files.Find(f => f.Name.Contains(name));

            toReturn = fileInfo;
            return fileInfo != null;
        }

        internal static void SaveFile()
        {
            if(CurrentLevel.levelName == "")
                SaveFileAs();

            FileInfo fileInfo;
            if (FileExists(CurrentLevel.levelName, ".json", out fileInfo))
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


