namespace Environment.Netsoft.WebView.Deserializers
{
	public class ActionDeserializer : IActionDeserializer
	{
		public object[] Deserialize(string value)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(value);
		}
	}
}
