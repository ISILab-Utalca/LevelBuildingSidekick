using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // add converters to serializer
            foreach(var converter in converters)
            {
                jsonSerializerSettings.Converters.Add(converter);
            }

            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new Vector2Converter());

            // generate json string
            var jsonString = JsonConvert.SerializeObject(
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
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // add converters to serializer
            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new Vector2Converter());

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

        public static void LoadConverters()
        {
            var converterTypes = Utility.Reflection.GetAllSubClassOf(typeof(JsonConverter));
            converterTypes.Where(c => c.);
        }


    }

    public class Vector3Converter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(Vector3);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = serializer.Deserialize<float[]>(reader);
            return new Vector3(values[0], values[1], values[2]);
            /*
            JObject obj = JObject.Load(reader);
            float x = (float)obj["x"];
            float y = (float)obj["y"];
            float z = (float)obj["z"];
            return new Vector3(x, y, z);*/
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;
            serializer.Serialize(writer, new float[] { vector.x, vector.y, vector.z });
            /*
            Vector3 v = (Vector3)value;
            JObject jo = new JObject();
            jo["x"] = v.x;
            jo["y"] = v.y;
            jo["z"] = v.z;
            jo.WriteTo(writer);*/
        }
    }

    public class Vector2Converter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(Vector2);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = serializer.Deserialize<float[]>(reader);
            return new Vector2(values[0], values[1]);
            /*
            JObject obj = JObject.Load(reader);
            float x = (float)obj["x"];
            float y = (float)obj["y"];
            return new Vector2(x, y);*/
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector2)value;
            serializer.Serialize(writer, new float[] { vector.x, vector.y});
            /*
            Vector2 v = (Vector2)value;
            JObject jo = new JObject();
            jo["x"] = v.x;
            jo["y"] = v.y;
            jo.WriteTo(writer);*/
        }
    }

    public class LBSTagConverter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(LBSTag);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            var tag = ScriptableObject.CreateInstance<LBSTag>();
            tag.value = value;
            return tag;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var tag = (LBSTag)value;
            writer.WriteValue(tag.value);
        }
    }
}
