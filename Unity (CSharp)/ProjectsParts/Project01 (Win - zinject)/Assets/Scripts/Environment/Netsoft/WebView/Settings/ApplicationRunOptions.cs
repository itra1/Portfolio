using System.Text;

namespace Environment.Netsoft.WebView.Settings
{
	public class ApplicationRunOptions : IApplicationRunOptions
	{
		/// <summary>
		/// Window ID
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// Closing application when there are no incoming packets pipe (msec)
		/// </summary>
		public int PipeTimeout { get; set; } = 0;

		/// <summary>
		/// Closing the application when there is no information on the material at the start (msec)
		/// </summary>
		public int InitializeTimeout { get; set; }

		public string MakeArgumentsrunApp()
		{
			StringBuilder sb = new();

			_ = sb.Append($" -id={Id}");

			_ = sb.Append($" -initializetimeout={PipeTimeout}");

			_ = sb.Append($" -pipetimeout={PipeTimeout}");

			return sb.ToString();
		}
	}
}
