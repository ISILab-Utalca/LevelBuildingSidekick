using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Utility
{
    public static class JSONDataManager
    {
        public static void SaveData<T>(string path, T data)
        {
            //Debug.Log("Saving: " + data);
            string dataPath = Application.persistentDataPath + '/' + path + ".json";
            string json = JsonUtility.ToJson(data);
            using StreamWriter writer = new StreamWriter(dataPath);
            writer.Write(json);
        }

        public static T LoadData<T>(string path)
        {
            string dataPath = Application.persistentDataPath + '/' + path;
            using StreamReader reader = new StreamReader(dataPath);

            //Debug.Log("Loading: " + dataPath);
            string json = reader.ReadToEnd();

            T data = JsonUtility.FromJson<T>(json);

            if (data == null)
            {
                Debug.LogWarning("Data in " + path + " is not of type" + typeof(T).ToString());
            }

            return data;
        }

        public static List<string> GetJSONFiles(string path)
        {
            string[] files = System.IO.Directory.GetFiles(path);
            //Debug.Log("Files: " + files.Length);
            List<string> jsonFiles = new List<string>();
            foreach (string s in files)
            {
                //Debug.Log("Path: " + s);
                if (s.EndsWith(".json"))
                {
                    string[] lines = s.Split('/');
                    jsonFiles.Add(lines[^1].Split('\\')[^1]);
                }
            }

            //Debug.Log("Json Files: " + jsonFiles.Count);

            return jsonFiles;
        }
    }

}
