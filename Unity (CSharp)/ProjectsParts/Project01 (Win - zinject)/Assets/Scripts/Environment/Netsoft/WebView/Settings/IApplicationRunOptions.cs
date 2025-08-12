namespace Environment.Netsoft.WebView.Settings
{
	public interface IApplicationRunOptions
	{
		public string Id { get; set; }
		public int PipeTimeout { get; set; }
		public int InitializeTimeout { get; set; }
		public string MakeArgumentsrunApp();
	}
}