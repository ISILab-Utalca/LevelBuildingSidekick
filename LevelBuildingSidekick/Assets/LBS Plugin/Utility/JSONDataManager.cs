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
        public static void SaveData<T>(string directoryName, string fileName, T data)
        {
            //Debug.Log("Saving: " + data);
            string directoryPath = Application.dataPath + '/' + directoryName;
            if(!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath =  directoryPath + '/' + fileName + ".json";
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }
            var jsonString = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            //string json = JsonUtility.ToJson(data);
            //Debug.Log("Save data to: " + dataPath);
            using StreamWriter writer = new StreamWriter(dataPath);
            writer.Write(jsonString);
            //writer.Write(json);
        }

        public static T LoadData<T>(string directoryName, string fileName)
        {
            string directoryPath = Application.dataPath + '/' + directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName;
            using StreamReader reader = new StreamReader(dataPath);

            //Debug.Log("Loading: " + dataPath);
            string json = reader.ReadToEnd();
            
            //T data = JsonUtility.FromJson<T>(json);
            var data = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented });

            if (data == null)
            {
                Debug.LogWarning("Data in " + fileName + " is not of type" + typeof(T).ToString());
            }

            return data;
        }

        public static List<string> GetJSONFiles(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //Debug.Log(path);
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
