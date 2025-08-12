namespace Environment.Netsoft.WebView.Serializer
{
	public interface IActionSerializer
	{
		public string Serialize(string key, object data = null);
	}
}
