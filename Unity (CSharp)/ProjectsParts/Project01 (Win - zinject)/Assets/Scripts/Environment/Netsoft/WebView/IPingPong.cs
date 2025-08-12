using System;
using Environment.Netsoft.WebView.Data;

namespace Environment.Netsoft.WebView
{
	public interface IPingPong
	{
		Action<PingData> OnStateDataChange { get; set; }
		void Process();

		void Terminate();
	}
}