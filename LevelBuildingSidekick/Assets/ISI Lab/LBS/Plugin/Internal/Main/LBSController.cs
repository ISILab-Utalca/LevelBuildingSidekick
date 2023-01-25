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
        private class LevelScriptable : GenericScriptable<LBSLevelData> { };
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
                    instance.level =  new LoadedLevel(new LBSLevelData(), "");
                }

                return instance.level;

            }
            set
            {
                var instance = LevelBackUp.Instance();
                instance.level = value;
            }
        }

        public static void LoadFile(string path)
        {
            var fileInfo = new System.IO.FileInfo(path);
            var data = Utility.JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName,fileInfo.Name);
            CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
        }

        public static LoadedLevel LoadFile()
        {
            var answer = EditorUtility.DisplayDialogComplex(
                   "The current file has not been saved",
                   "if you open a file the progress in the current document will be lost, are you sure to continue?",
                   "save",
                   "discard",
                   "cancel");
            string path;
            FileInfo fileInfo;
            LBSLevelData data;
            switch (answer)
            {
                case 0: // Save
                    SaveFile();
                    path = EditorUtility.OpenFilePanel("Load level data", "", "lbs");
                    fileInfo = new System.IO.FileInfo(path);
                    data = Utility.JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
                    return CurrentLevel;

                case 1: // Discard
                    path = EditorUtility.OpenFilePanel("Load level data", "", "lbs");
                    if (path == "")
                        return null;
                    fileInfo = new System.IO.FileInfo(path);
                    data = Utility.JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
                    return CurrentLevel;
            }
            return null;
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

        public static void SaveFile()
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
                Utility.JSONDataManager.SaveData(fileInfo.DirectoryName, fileInfo.Name, CurrentLevel.data);
            }
        }

        public static void SaveFileAs()
        {
            var path = "";
            var fileInfo = CurrentLevel.FileInfo;
            if (CurrentLevel.FileInfo != null)
            {
                
                path = EditorUtility.SaveFilePanel(
                    "Save level",
                    (fileInfo.Exists) ? fileInfo.DirectoryName : Application.dataPath,
                    (fileInfo.Exists) ? fileInfo.Name : defaultName + ".lbs",
                    "lbs");
            }
            else
            {
                path = EditorUtility.SaveFilePanel(
                    "Save level",
                    Application.dataPath,
                    defaultName + ".lbs",
                    "lbs");
                CurrentLevel.fullName = path;
            }

            if (path != "")
            {
                Debug.Log("Save file on: '" + path + "'.");
                //Debug.Log(fileInfo + " - " + CurrentLevel);
                Utility.JSONDataManager.SaveData(CurrentLevel.FileInfo.DirectoryName, CurrentLevel.FileInfo.Name, CurrentLevel.data);
                LevelBackUp.Instance().level.fullName = path;
            }
        }

        public static LoadedLevel CreateNewLevel(string levelName, Vector3 size)
        {
            var data = new LBSLevelData();
            var loaded = new LoadedLevel(data, null);
            //data.Size = size;
            //data.AddRepresentation(new LBSGraphData());
            CurrentLevel = loaded;
            return CurrentLevel;
        }

        public static void ShowLevelInspector()
        {
            var s = ScriptableObject.CreateInstance<LevelScriptable>();
            s.data = CurrentLevel.data;
            Selection.SetActiveObjectWithContext(s, s);

        }

    }

    
}


