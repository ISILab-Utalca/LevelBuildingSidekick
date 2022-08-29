using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Utility
{
    public static class DirectoryTools
    {
        public static T SearchAssetByName<T>(string name)
        {
            var guids = AssetDatabase.FindAssets(name);
            object obj = null;
            foreach (var guid in guids)
            {
                obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T));
                if(obj != null)
                {
                    break;
                }
            }

            return (T)obj;
        }

        public static List<T> GetScriptablesByType<T>() where T : ScriptableObject
        {
            List<T> toReturn = new List<T>();
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).ToString());
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                toReturn.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }
            return toReturn;
        }

        public static List<FileInfo> GetAllFilesByExtension(string extension, string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.Log("the directory named " + path + " does not exist");
                return null;
            }

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            var files = Utility.DirectoryTools.GetAllFilesByExtension(extension, dir);
            return files;
        }

        public static List<FileInfo> GetAllFilesByExtension(string extension, DirectoryInfo dir)
        {
            var directory = dir.GetDirectories();
            var files = dir.GetFiles();

            var toReturn = new List<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].FullName.EndsWith(extension))
                {
                    toReturn.Add(files[i]);
                }
            }

            var dirs = dir.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                toReturn = toReturn.Concat(GetAllFilesByExtension(extension, dirs[i])).ToList();
            }
            return toReturn;
        }
    }
}

