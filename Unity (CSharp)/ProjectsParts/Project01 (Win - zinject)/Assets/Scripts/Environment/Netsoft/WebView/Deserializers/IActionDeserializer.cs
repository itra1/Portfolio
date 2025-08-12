namespace Environment.Netsoft.WebView.Deserializers
{
	public interface IActionDeserializer
	{
		public object[] Deserialize(string value);
	}
}
