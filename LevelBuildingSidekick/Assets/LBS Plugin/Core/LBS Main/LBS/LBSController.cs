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

        internal static void LoadFile()
        {
            // aqui preguntar si exiten cambios entre la version abierta y la guardada
            // si no es nulo y exiten diferencias entre el abierto y la guardada
            // abrir un "Dialog" que pregunte si esta seguro de cargar otro archivo
            // y que si lo hace se perdera el actual;
            if (backUp != null && true) 
            {
                
                var diag = EditorUtility.DisplayDialog(
                    "The current file has not been saved", 
                    "if you open a file the progress in the current document will be lost, are you sure to continue?",
                    "continue",
                    "cancel");

                if(diag)
                    SaveFile();
            }



        }

        private static bool FileExists(string name, string extension, out FileInfo toReturn)
        {
            var path = Application.dataPath;
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            var files = Utility.JSONDataManager.GetAllFilesByExtencion(extension, dir);

            var fileInfo = files.Find(f => f.Name.Contains(name));

            toReturn = fileInfo;
            return fileInfo != null;
        }

        private static bool FileExists(string name,string extension)
        {
            FileInfo useless;
            return FileExists(name, extension, out useless);
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


