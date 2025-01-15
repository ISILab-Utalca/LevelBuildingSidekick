using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using ISILab.JsonNet;

namespace ISILab.LBS
{
    public static class LBSController // FIX: Change name to a better name
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
                    level = ScriptableObject.CreateInstance<LoadedLevel>();
                    level.data = new LBSLevelData();
                    level.fullName = "";
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
            var data = JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName,fileInfo.Name);
            CurrentLevel = LoadedLevel.CreateInstance(data, fileInfo.FullName);
            CurrentLevel.data.Reload();
            OnLoadLevel?.Invoke(CurrentLevel.data);
            LBSMainWindow.MessageNotify("The level '\" + fileInfo.Name + \"' has been loaded successfully.");
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
                    data = JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = LoadedLevel.CreateInstance(data, fileInfo.FullName);
                    CurrentLevel.data.Reload();
                    return CurrentLevel;

                case 1: // Discard
                    
                    LBSMainWindow.MessageNotify("Level discarded.");
                    
                    path = EditorUtility.OpenFilePanel("Load level data", "", "lbs");
                    if (path == "")
                        return null;
                    fileInfo = new System.IO.FileInfo(path);
                    data = JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = LoadedLevel.CreateInstance(data, fileInfo.FullName);
                    CurrentLevel.data.Reload();
                    OnLoadLevel?.Invoke(CurrentLevel.data);
                    return CurrentLevel;

                case 2: //do nothing
                default:
                    return null; 
            }
        }

        /// <summary>
        /// Saves the current level data to the file it was loaded from,
        /// or prompts the user to save it as a new file if it has not been
        /// saved before.
        /// </summary>
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
                JSONDataManager.SaveData(fileInfo.DirectoryName, fileInfo.Name, CurrentLevel.data);
            }
            LBSMainWindow.MessageNotify("The file has been saved.");
        }

        /// <summary>
        /// Saves the current level data to a new file,
        /// prompting the user to choose the file path.
        /// </summary>
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
                JSONDataManager.SaveData(directory, filename, CurrentLevel.data);
                LBS.loadedLevel.fullName = path;
            }
            
            LBSMainWindow.MessageNotify("The file has been saved.");
        }

        /// <summary>
        /// Creates a new level with the specified name and size.
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static LoadedLevel CreateNewLevel(string levelName = "new file")
        {
            var data = new LBSLevelData();
            var loaded = LoadedLevel.CreateInstance(data, null);
            CurrentLevel = loaded;
            CurrentLevel.data.Reload();
            return CurrentLevel;
            
        }
    }
}