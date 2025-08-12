namespace Core.Options
{
	/// <summary>
	/// Включает в себя свойства из устаревших "GameOptions" и "ConfigManager"
	/// Данный интерфейс обеспечивает доступ к свойствам только для записи данных
	/// </summary>
	public interface IApplicationOptionsSetter
	{
		string CustomServer { set; }
		string ServerToken { set; }
		bool IsStateSendingAllowed { set; }
		bool IsLocalSslIgnored { set; }
		bool IsExtendedSslIgnored { set; }
		/// <summary>
		/// Do not block on startup
		/// </summary>
		bool IsUnlockedAtStart { set; }
		bool IsPresetRestoredAtStart { set; }
		bool IsDevServerEnabled { set; }
		bool IsManagersLogEnabled { set; }
		bool IsConsoleEnabled { set; }
		bool IsPdfRenderingAsPicture { set; }
		bool IsFpsCounterEnabled { set; }
		bool? IsPreviewEnabled { set; }
		bool IsOnTopByDefault { set; }
		bool IsLoadingIndicatorEnabled { set; }
		bool IsRenderStreamingEnabled { set; }
		bool IsRenderStreamingStunUsing { set; }
		string RenderStreamingUrl { set; }
		float? ScreenBorderUp { set; }
		float? ScreenBorderRight { set; }
		float? ScreenBorderDown { set; }
		float? ScreenBorderLeft { set; }
		int? WidgetUpdatePeriod { set; }
		float? MinFloatingWindowSizeX { set; }
		float? MinFloatingWindowSizeY { set; }
		float? DefaultCustomFloatingWindowSizeX { set; }
		float? DefaultCustomFloatingWindowSizeY { set; }
		float? ScreenCornerFloatingWindowSizeX { set; }
		float? ScreenCornerFloatingWindowSizeY { set; }
		float? OneThirdScreenFloatingWindowSizeX { set; }
		float? OneThirdScreenFloatingWindowSizeY { set; }
		bool IsSumAdaptiveModeActive { set; }
		int? SumAdaptiveModeColumn { set; }
		bool IsMSOfficeUsed { set; }
		bool QtBrowser { set; }
	}
}