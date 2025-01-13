using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System;
using ISILab.Extensions;

namespace ISILab.JsonNet
{
    public static class JSONDataManager
    {
        public static List<JsonConverter> converters = new List<JsonConverter>();

        /// <summary>
        /// Save data in a file in the path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private static void SaveData<T>(string path, T data)
        {
            // generate serializer setting
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize, //Esto arregla un problema de serialización de LBSCharacteristics, pero puede que cause problemas luego
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Get all the derived types of JsonConverter and add them to the settings
            var derives = (typeof(JsonConverter)).GetDerivedTypes();
            foreach (var derive in derives)
            {
                if (derive.GetConstructors().Any(c => c.GetParameters().Count() <= 0))
                {
                    jsonSerializerSettings.Converters.Add((JsonConverter)Activator.CreateInstance(derive));
                }
            }

            // generate json string

            Debug.Log("test");
            string jsonString = "ERROR";
            jsonString = JsonConvert.SerializeObject(
                data,
                jsonSerializerSettings
                );

            // write json in a file
            using StreamWriter writer = new StreamWriter(path);
            writer.Write(jsonString);
        }

        /// <summary>
        /// Save data in a file in the path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryName"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
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

        /// <summary>
        /// Save data in a file in the path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryName"></param>
        /// <param name="fileName"></param>
        /// <param name="format"></param>
        /// <param name="data"></param>
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

        /// <summary>
        /// Load data from a file in the path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
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

            // Get all the derived types of JsonConverter and add them to the settings
            var derives = (typeof(JsonConverter)).GetDerivedTypes();
            foreach (var derive in derives)
            {
                if (derive.GetConstructors().Any(c => c.GetParameters().Count() <= 0))
                {
                    jsonSerializerSettings.Converters.Add((JsonConverter)Activator.CreateInstance(derive));
                }
            }

            // generate data from string
            var data = JsonConvert.DeserializeObject<T>(
                json,
                jsonSerializerSettings
                );

            if (data == null)
                Debug.LogWarning("Data in " + path + " is not of type " + typeof(T).ToString());

            return data;
        }

        /// <summary>
        /// Load data from a file in the path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Load data from a file in the path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryName"></param>
        /// <param name="fileName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get all the json files in a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetJSONFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                return null;
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
