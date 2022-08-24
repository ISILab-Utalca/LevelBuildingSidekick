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
            var jsonString = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            using StreamWriter writer = new StreamWriter(path);
            writer.Write(jsonString);
        }

        public static void SaveData<T>(string directoryName, string fileName, T data)
        {
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

            SaveData(dataPath,data);
        }

        public static T LoadData<T>(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented });

            if (data == null)
                Debug.LogWarning("Data in " + path + " is not of type" + typeof(T).ToString());

            return data;
        }

        public static T LoadData<T>(string directoryName, string fileName)
        {
            string directoryPath = Application.dataPath + '/' + directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName;

            return LoadData<T>(dataPath);
        }

        public static List<string> GetJSONFiles(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = System.IO.Directory.GetFiles(path);
            List<string> jsonFiles = new List<string>();
            foreach (string s in files)
            {
                if (s.EndsWith(".json"))
                {
                    string[] lines = s.Split('/');
                    jsonFiles.Add(lines[^1].Split('\\')[^1]);
                }
            }

            return jsonFiles;
        }


    }

}
