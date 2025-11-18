using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class StringToValueConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(float) || objectType == typeof(int) || objectType == typeof(long);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            string value = reader.Value.ToString();
            
            if (objectType == typeof(float))
                return float.Parse(value);
            if (objectType == typeof(int))
                return int.Parse(value);
            if (objectType == typeof(long))
                return long.Parse(value);
        }
        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

}
