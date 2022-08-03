using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Utility
{
    public static class JSONDataManager
    {
        public static void SaveData<T>(string path, T data)
        {
            //Debug.Log("Saving: " + data);
            string dataPath = Application.persistentDataPath + '/' + path + ".json";
            var jsonString = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            //string json = JsonUtility.ToJson(data);
            using StreamWriter writer = new StreamWriter(dataPath);
            writer.Write(jsonString);
            //writer.Write(json);
        }

        public static T LoadData<T>(string path)
        {
            string dataPath = Application.persistentDataPath + '/' + path;
            using StreamReader reader = new StreamReader(dataPath);

            //Debug.Log("Loading: " + dataPath);
            string json = reader.ReadToEnd();
            
            //T data = JsonUtility.FromJson<T>(json);
            var data = JsonConvert.DeserializeObject<T>(json);

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
            //Debug.Log("Path: " + path);
            List<string> jsonFiles = new List<string>();
            foreach (string s in files)
            {
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
