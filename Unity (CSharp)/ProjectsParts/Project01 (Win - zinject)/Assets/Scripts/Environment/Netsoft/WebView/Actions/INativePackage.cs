using UnityEngine.Events;

namespace Environment.Netsoft.WebView.Actions
{
	public interface INativePackage
	{
		string Action { get; set; }
		object Data { get; set; }
		UnityAction<string, object> OnComplete { get; set; }
	}
}