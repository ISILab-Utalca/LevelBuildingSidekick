using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using LBS.Graph;
using System.Text;
using LBS.Representation.TileMap;
using Utility;
using LBS.Schema;
using System.Linq;

namespace LBS
{
    public static class Globals
    {

    }

    // change name "LBSController" to "LBS" or "LBSCore" or "LBSMain" or "LBSManager" (!) 
    // esta clase podria ser estatica completamente (??)
    public static class LBSController 
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
                    path = EditorUtility.OpenFilePanel("Load level data", "", "json");
                    fileInfo = new System.IO.FileInfo(path);
                    data = Utility.JSONDataManager.LoadData<LevelData>(path);
                    CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
                    break;
                case 1: // cancel
                    path = EditorUtility.OpenFilePanel("Load level data", "", "json");
                    if (path == "")
                        return;
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
            {
                SaveFileAs();
            }
            else if (!fileInfo.Exists)
            {
                SaveFileAs();
            }
            else
            {
                Utility.JSONDataManager.SaveData(fileInfo.FullName, CurrentLevel.data);
            }
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
                LevelBackUp.Instance().level.fullName = path;
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


