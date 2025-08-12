using UnityEngine;

public class AppLog
{
	public static void Log(object messege)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.Log(messege);
#endif
	}
	public static void Log(object message, Object context)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.Log(message, context);
#endif
	}
	public static void LogError(object messege)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogError(messege);
#endif
	}
	public static void LogError(System.Exception messege)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogError($"Error {messege.Message} {messege.StackTrace}");
#endif
	}
	public static void LogError(object message, Object context)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogError(message, context);
#endif
	}
	public static void LogWarning(object messege)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogWarning(messege);
#endif
	}
	public static void LogWarning(object message, Object context)
	{
#if UNITY_EDITOR || !LOG_EDITOR_ONLY
		Debug.LogWarning(message, context);
#endif
	}
}
