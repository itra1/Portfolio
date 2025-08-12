using System.Collections;

using UnityEngine;


public static class AppConfig
{
	private static int? _sessionId = int.MaxValue;
	public static bool IsLog { get; set; } = false;
	public static bool DisableAudio { get; set; } = false;
	public static bool DevServer { get; set; } = false;
	public static bool IsDevApp { get; set; } = false;
	public static string CustomServer { get; set; } = "";
	public static string CustomServerWS { get; set; } = "";
	/// <summary>
	/// Флаг блокироваки открытия столов
	/// </summary>
	public static bool IsLockedOpenTable { get; set; } = false;
	public static int? SessinId { get; set; } = int.MaxValue;
	public static ulong? TableExe { get; set; } = null;
	public static bool IsLobby => TableExe == null;
	public static bool ActiveCashier { get; set; } = false;
}