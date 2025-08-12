using UnityEngine;

namespace Environment.Netsoft.WebView.Settings
{
	[CreateAssetMenu(fileName = "NsWebViewSettings", menuName = "Settings/NsWebViewSettings")]
	public class NetsofrWebViewSettings : ScriptableObject, INetsofrWebViewSettings
	{
		[SerializeField, Tooltip("Closing the application when there are no incoming packets {msec}")]
		private int _pipeTimeout;
		[SerializeField, Tooltip("Closing the application when the first package is missing (msec)")]
		private int _initializeTimeout;

		public int PipeTimeout => _pipeTimeout;
		public int InitializeTimeout => _initializeTimeout;
	}
}
