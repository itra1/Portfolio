using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace it.Game
{
  /// <summary>
  /// Переопределенный логгер
  /// </summary>
  public static class Logger
  {

	 /// <summary>
	 /// Вывод простого лога
	 /// </summary>
	 /// <param name="log">Текст сообщения</param>
	 public static void Log(string log)
	 {
		if (ProjectSettings.PrintDebug)
		  Debug.Log(log);

	 }

	 /// <summary>
	 /// вывод ошибки
	 /// </summary>
	 /// <param name="log">Текст сообщения</param>

	 public static void LogError(string log)
	 {
		if (ProjectSettings.PrintDebug)
		  Debug.LogError(log);
		return;
	 }
	 /// <summary>
	 /// Вывод предупреждения
	 /// </summary>
	 /// <param name="log">Текст сообщения</param>
	 public static void LogWarning(string log)
	 {
		if (ProjectSettings.PrintDebug)
		  Debug.LogWarning(log);
	 }

  }
}