namespace Core.Options
{
	/// <summary>
	/// Включает в себя свойства из устаревших "GameOptions" и "ConfigManager"
	/// Данный интерфейс обеспечивает доступ к свойствам только для чтения данных
	/// </summary>
	public interface IApplicationOptions
	{
		string CustomServer { get; }
		string ServerToken { get; }
		bool IsStateSendingAllowed { get; }
		bool IsLocalSslIgnored { get; }
		bool IsExtendedSslIgnored { get; }
		bool IsUnlockedAtStart { get; }
		bool IsPresetRestoredAtStart { get; }
		bool IsDevServerEnabled { get; }
		bool IsManagersLogEnabled { get; }
		bool IsConsoleEnabled { get; }
		bool IsPdfRenderingAsPicture { get; }
		bool IsFpsCounterEnabled { get; }
		bool? IsPreviewEnabled { get; }
		bool IsOnTopByDefault { get; }
		bool IsLoadingIndicatorEnabled { get; }
		bool IsRenderStreamingEnabled { get; }
		bool IsRenderStreamingStunUsing { get; }
		string RenderStreamingUrl { get; }
		float? ScreenBorderUp { get; }
		float? ScreenBorderRight { get; }
		float? ScreenBorderDown { get; }
		float? ScreenBorderLeft { get; }
		int? WidgetUpdatePeriod { get; }
		float? MinFloatingWindowSizeX { get; }
		float? MinFloatingWindowSizeY { get; }
		float? DefaultCustomFloatingWindowSizeX { get; }
		float? DefaultCustomFloatingWindowSizeY { get; }
		float? ScreenCornerFloatingWindowSizeX { get; }
		float? ScreenCornerFloatingWindowSizeY { get; }
		float? OneThirdScreenFloatingWindowSizeX { get; }
		float? OneThirdScreenFloatingWindowSizeY { get; }
		bool IsSumAdaptiveModeActive { get; }
		int? SumAdaptiveModeColumn { get; }
		bool IsMSOfficeUsed { get; }
		bool QtBrowser { get; }
	}
}