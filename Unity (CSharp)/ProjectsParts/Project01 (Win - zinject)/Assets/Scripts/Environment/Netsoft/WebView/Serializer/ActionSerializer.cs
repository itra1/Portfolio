namespace Environment.Netsoft.WebView.Serializer
{
	public class ActionSerializer : IActionSerializer
	{
		public string Serialize(string key, object data = null)
		{
			object[] sendData = data != null
			? new object[] { key, data }
			: new object[] { key };

			return Newtonsoft.Json.JsonConvert.SerializeObject(sendData);
		}
	}
}
