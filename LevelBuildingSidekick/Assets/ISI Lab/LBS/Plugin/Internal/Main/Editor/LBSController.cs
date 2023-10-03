using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Text;
using Utility;
using System.Linq;

namespace LBS
{
    // change name "LBSController" to "LBS" or "LBSCore" or "LBSMain" or "LBSManager" (!) 
    // esta clase podria ser estatica completamente (??)
    public static class LBSController 
    {
        private static readonly string defaultName = "New file";

        public static Action<LBSLevelData> OnLoadLevel;
        public static Action<LBSLevelData> OnSaveLevel;

        public static LoadedLevel CurrentLevel
        {
            get
            {
                var level = LBS.loadedLevel;
                if (level == null)
                {
                    level =  new LoadedLevel(new LBSLevelData(), "");
                }
                return level;
            }
            set
            {
                LBS.loadedLevel = value;
            }
        }

        /// <summary>
        /// Loads a file from the specified path and initializes a new level based on the data obtained.
        /// </summary>
        /// <param name="path">The path of the file to be loaded.</param>
        public static void LoadFile(string path)
        {
            var fileInfo = new System.IO.FileInfo(path);
            var data = Utility.JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName,fileInfo.Name);
            CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
            CurrentLevel.data.Reload();
            OnLoadLevel?.Invoke(CurrentLevel.data);
            Debug.Log("[ISI Lab]: The level '" + fileInfo.Name + "' has been loaded successfully.");
        }

        /// <summary>
        /// Prompts the user with a dialog to open a file and loads the level data based on the user's choice.
        /// </summary>
        /// <returns>
        /// The loaded level as a <see cref="LoadedLevel"/> object, 
        /// or null if the operation is canceled or no file is selected.
        /// </returns>
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
                    CurrentLevel.data.Reload();
                    return CurrentLevel;

                case 1: // Discard
                    path = EditorUtility.OpenFilePanel("Load level data", "", "lbs");
                    if (path == "")
                        return null;
                    fileInfo = new System.IO.FileInfo(path);
                    data = Utility.JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = new LoadedLevel(data, fileInfo.FullName);
                    CurrentLevel.data.Reload();
                    OnLoadLevel?.Invoke(CurrentLevel.data);
                    return CurrentLevel;

                case 2: //do nothing
                default:
                    return null; 
            }
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
                var parts = path.Split("/");
                var filename = parts[^1];
                var directory = path.Substring(0,path.Length - filename.Length);
                Debug.Log("Save file on: '" + directory + filename + "'.");
                Utility.JSONDataManager.SaveData(directory, filename, CurrentLevel.data);
                LBS.loadedLevel.fullName = path;
            }
        }

        public static LoadedLevel CreateNewLevel(string levelName, Vector3 size)
        {
            var data = new LBSLevelData();
            var loaded = new LoadedLevel(data, null);
            //data.Size = size;
            //data.AddRepresentation(new LBSGraphData());
            CurrentLevel = loaded;
            CurrentLevel.data.Reload();
            return CurrentLevel;
        }
    }
}