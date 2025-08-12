using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[Serializable]
public abstract class Jsonable
{
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static T ToObject<T>(string json)
    {
        var errors = new List<string>();
        // return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            //NullValueHandling = NullValueHandling.Ignore,
            Error = delegate(object sender, ErrorEventArgs args)
            {
                errors.Add(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
                it.Logger.LogError(args.ErrorContext.Error.Message);
                it.Logger.LogError(args.ErrorContext.Path);
            }
        });
        //return JsonUtility.FromJson<T>(json);
    }
}