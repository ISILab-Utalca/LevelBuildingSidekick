using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSTagConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(LBSIdentifier);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        var value = serializer.Deserialize(reader);
        return JsonConvert.DeserializeObject<LBSIdentifier>(value.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var tag = (LBSIdentifier)value;

        writer.WriteStartObject();
        writer.WritePropertyName("label");
        writer.WriteValue(tag.Label);
        writer.WriteEndObject();

    }
}