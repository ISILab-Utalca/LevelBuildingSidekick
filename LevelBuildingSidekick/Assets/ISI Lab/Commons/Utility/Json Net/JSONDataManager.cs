using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Utility
{
    public static class JSONDataManager
    {
        public static List<JsonConverter> converters = new List<JsonConverter>();

        private static void SaveData<T>(string path, T data)
        {
            // generate serializer setting
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // add converters to serializer
            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new Vector2Converter());
            jsonSerializerSettings.Converters.Add(new ColorConverter());

            // generate json string
            string jsonString = "ERROR";
            jsonString = JsonConvert.SerializeObject(
                data,
                jsonSerializerSettings
                );

            // write json in a file
            using StreamWriter writer = new StreamWriter(path);
            writer.Write(jsonString);
        }

        public static void SaveData<T>(string directoryName, string fileName, T data)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName;
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }

            SaveData(dataPath, data);
        }

        public static void SaveData<T>(string directoryName, string fileName, string format, T data)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName + "." + format;
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }

            SaveData(dataPath, data);
        }

        private static T LoadData<T>(string path)
        {
            // read file and obtain json string
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();

            // generate serializer setting
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // add converters to serializer
            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new Vector2Converter());
            jsonSerializerSettings.Converters.Add(new ColorConverter());

            // generate data from string
            var data = JsonConvert.DeserializeObject<T>(
                json,
                jsonSerializerSettings
                );

            if (data == null)
                Debug.LogWarning("Data in " + path + " is not of type " + typeof(T).ToString());

            return data;
        }

        public static T LoadData<T>(string directoryName, string fileName)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName;

            return LoadData<T>(dataPath);
        }

        public static T LoadData<T>(string directoryName, string fileName, string format)
        {
            string directoryPath = directoryName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataPath = directoryPath + '/' + fileName + "." + format;

            return LoadData<T>(dataPath);
        }

        public static List<string> GetJSONFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                return null;
                //return new List<string>(); // (??) return empty list
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
