using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Garilla.Platform.WebGL
{
	public class WebGLCopyPastle : MonoBehaviour
	{
		private static WebGLCopyPastle _instance;

		private UnityEngine.Events.UnityAction<string> OnPastAction;
		private UnityEngine.Events.UnityAction<bool> OnCopyAction;

		public static void Init(){

			GameObject go = new GameObject("WebGLCopyPastle");
			_instance = go.AddComponent<WebGLCopyPastle>();

		}

		private void Awake()
		{
			_instance = this;
		}

		public static void CopyText(string text, UnityEngine.Events.UnityAction<bool> OnComplete = null)
		{
			if (_instance == null) Init();
			_instance.OnCopyAction = OnComplete;
			myCopyToClipboard(text);
		}

		public static void PastText(UnityEngine.Events.UnityAction<string> OnComplete)
		{
			if (_instance == null) Init();

			_instance.OnPastAction = OnComplete;
			myPastFromClipboard();
		}

		public void OnCopyCallback(string succes)
		{
			OnCopyAction?.Invoke(succes == "1" ? true : false);
		}
		public void OnPastleCallback(string pastleText)
		{
			OnPastAction?.Invoke(pastleText);
		}

		[DllImport("__Internal")]
		private static extern void myCopyToClipboard(string text);

		[DllImport("__Internal")]
		private static extern void myPastFromClipboard();

	}
}