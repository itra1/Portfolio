using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Json
{
  /// <summary>
  /// Обвертка для JsonConvernera
  /// </summary>
  public static class JsonConvert
  {

	 public static string Serializable(object sourceObject)
	 {
		return Newtonsoft.Json.JsonConvert.SerializeObject(sourceObject);
	 }

	 public static T Deserializable<T>(string json)
	 {
		return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
	 }

  }
}