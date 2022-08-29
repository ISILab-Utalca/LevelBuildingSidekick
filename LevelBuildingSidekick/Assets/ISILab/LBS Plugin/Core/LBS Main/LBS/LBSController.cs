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
    // change name "LBSController" to "LBS" or "LBSCore" or "LBSMain" (!)
    // esta clase podria ser estatica completamente (??)
    public class LBSController 
    {
        #region InspectorDrawer
        private class LevelScriptable : GenericScriptable<LevelData> { };
        [CustomEditor(typeof(LevelScriptable)),CanEditMultipleObjects]
        private class LevelScriptableEditor : GenericScriptableEditor { };
        #endregion

        //private static LevelBackUp backUp;

        private static readonly string defaultName = "New file";

        public static LoadedLevel CurrentLevel
        {
            get
            {
                var instance = LevelBackUp.Instance();
                //Debug.Log("A: "+ instance.level.data);
                //var reps = instance.level.data.representations;
                //if (reps.Count > 0) reps[0].Print(); else Debug.Log("EE");
                //Debug.Log("B: " + instance.level.fullName);
                if (instance.level == null)
                {
                    instance.level =  new LoadedLevel(new LevelData(), "");
                }

                return instance.level;

            }
            set
            {
                var instance = LevelBackUp.Instance();
                instance.level = value;
            }
        }

        internal static void LoadFile(string path)
        {
            var fileInfo = new System.IO.FileInfo(path);
            var data = Utility.JSONDataManager.LoadData<LevelData>(path);
            CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
        }

        internal static void LoadFile()
        {
            var answer = EditorUtility.DisplayDialogComplex(
                   "The current file has not been saved",
                   "if you open a file the progress in the current document will be lost, are you sure to continue?",
                   "save", // ok
                   "discard", // cancel
                   "cancel"); // alt
            string path;
            FileInfo fileInfo;
            LevelData data;
            switch (answer)
            {
                case 0: // ok
                    SaveFile();
                    path = EditorUtility.OpenFilePanel("Load level data", "", ".json");
                    fileInfo = new System.IO.FileInfo(path);
                    data = Utility.JSONDataManager.LoadData<LevelData>(path);
                    CurrentLevel = new LoadedLevel(data,fileInfo.FullName);
                    break;
                case 1: // cancel
                    path = EditorUtility.OpenFilePanel("Load level data", "", ".json");
                    fileInfo = new System.IO.FileInfo(path);
                    data = Utility.JSONDataManager.LoadData<LevelData>(path);
                    CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
                    break;
                case 2: // alt
                    // do nothing
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        public static bool FileExists(string name, string extension, out FileInfo toReturn)
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
            var fileInfo = CurrentLevel.FileInfo;
            if (fileInfo == null)
                SaveFileAs();

            if (!fileInfo.Exists)
                SaveFileAs();

            Utility.JSONDataManager.SaveData(fileInfo.FullName, CurrentLevel.data);

            //if(CurrentLevel.levelName == "")
            //SaveFileAs();

            //FileInfo fileInfo;
            //if (FileExists(CurrentLevel.levelName, ".json", out fileInfo))
            //{
            //    Utility.JSONDataManager.SaveData(fileInfo.FullName, LBSController.CurrentLevel);
            //}
            //else
            //{
            //    SaveFileAs();
            //}

        }

        internal static void SaveFileAs()
        {
            var path = "";
            if (CurrentLevel.FileInfo != null)
            {
                var fileInfo = CurrentLevel.FileInfo;
                path = EditorUtility.SaveFilePanel(
                    "Save level",
                    (fileInfo.Exists) ? fileInfo.DirectoryName : Application.dataPath,
                    (fileInfo.Exists) ? fileInfo.Name : defaultName + ".json",
                    "json");
            }
            else
            {
                path = EditorUtility.SaveFilePanel(
                    "Save level",
                    Application.dataPath,
                    defaultName + ".json",
                    "json");
            }

            if (path != "")
            {
                Debug.Log("Save file on: '" + path + "'.");
                Utility.JSONDataManager.SaveData(path, CurrentLevel.data);
            }
        }

        public static LevelData CreateNewLevel(string levelName, Vector3 size)
        {
            var data = new LevelData();
            var loaded = new LoadedLevel(data, null);
            data.Size = size;
            data.AddRepresentation(new LBSGraphData());
            CurrentLevel = loaded;
            return data;
        }

        public static void ShowLevelInspector()
        {
            var s = ScriptableObject.CreateInstance<LevelScriptable>();
            s.data = CurrentLevel.data;
            Selection.SetActiveObjectWithContext(s, s);

        }

    }

    
}


