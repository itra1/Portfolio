using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class BestHTTPEncoder : BestHTTP.SocketIO.JsonEncoders.IJsonEncoder
{
	public List<object> Decode(string obj)
	{
		return new List<object> { obj };
	}

	public string Encode(List<object> obj)
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
	}
}