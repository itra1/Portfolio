namespace Environment.Netsoft.WebView.Data
{
	public class WebViewStateData
	{
		public string Url;
		public string LoadStatus;
		public double Zoom;
		public SizeStruct Scroll;
		public SizeStruct ContentSize;

		public class SizeStruct
		{
			public double x;
			public double y;
		}
	}
}
