using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;


public class WebGLController : MonoBehaviour
{
	public static WebGLController Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{

#if UNITY_WEBGL && !UNITY_EDITOR
		StartCoroutine(StartClearingConsole());
#endif

	}

	public static string GetCurrentURL()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		return getCurrentUrl();
#else
		return "";
#endif
	}

	public static void PrintCurrentURL()
	{
#if UNITY_WEBGL
		Debug.Log("Current url " + GetCurrentURL());
#endif
	}

#if UNITY_WEBGL

	IEnumerator StartClearingConsole()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (!AppConfig.IsLog)
				clearConsole();
		}
	}

	[DllImport("__Internal")]
	private static extern string getCurrentUrl();

	[DllImport("__Internal")]
	private static extern void clearConsole();
#endif
}