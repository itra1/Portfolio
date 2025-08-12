using System;
using UnityEngine;

public static class AppLog
{
	public static event Action<string> OnLog;

	public static void LogConsole(object message)
	{
		OnLog?.Invoke(message.ToString());
	}

	public static void Log(object message)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.Log($"{DateTime.Now.ToString()} {message}");
#endif
	}
	public static void LogError(object message)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogError($"{DateTime.Now.ToString()} {message}");
#endif
	}
	public static void LogWarning(object message)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogWarning($"{DateTime.Now.ToString()} {message}");
#endif
	}
}
