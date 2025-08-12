using UnityEngine.Events;

namespace Environment.Netsoft.WebView.Actions
{
	public class NativePackage : INativePackage
	{
		public string Action { get; set; }
		public object Data { get; set; }
		public UnityAction<string, object> OnComplete { get; set; }
	}
}
