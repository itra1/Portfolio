using System.Text;
using Core.Common.Consts;

namespace Core.Options
{
	/// <summary>
	/// Включает в себя свойства из устаревших "GameOptions" и "ConfigManager"
	/// </summary>
	public class ApplicationOptions : IApplicationOptionsInfo, IApplicationOptionsSetter
	{
		public string CustomServer { get; set; }
		public string ServerToken { get; set; }
		public bool IsStateSendingAllowed { get; set; }
		public bool IsLocalSslIgnored { get; set; }
		public bool IsExtendedSslIgnored { get; set; }
		public bool IsUnlockedAtStart { get; set; }
		public bool IsPresetRestoredAtStart { get; set; }
		public bool IsDevServerEnabled { get; set; }
		public bool IsManagersLogEnabled { get; set; }
		public bool IsConsoleEnabled { get; set; }
		public bool IsPdfRenderingAsPicture { get; set; }
		public bool IsFpsCounterEnabled { get; set; }
		public bool? IsPreviewEnabled { get; set; }
		public bool IsOnTopByDefault { get; set; }
		public bool IsLoadingIndicatorEnabled { get; set; }
		public bool IsRenderStreamingEnabled { get; set; }
		public bool IsRenderStreamingStunUsing { get; set; }
		public string RenderStreamingUrl { get; set; }
		public float? ScreenBorderUp { get; set; }
		public float? ScreenBorderRight { get; set; }
		public float? ScreenBorderDown { get; set; }
		public float? ScreenBorderLeft { get; set; }
		public int? WidgetUpdatePeriod { get; set; }
		public float? MinFloatingWindowSizeX { get; set; }
		public float? MinFloatingWindowSizeY { get; set; }
		public float? DefaultCustomFloatingWindowSizeX { get; set; }
		public float? DefaultCustomFloatingWindowSizeY { get; set; }
		public float? ScreenCornerFloatingWindowSizeX { get; set; }
		public float? ScreenCornerFloatingWindowSizeY { get; set; }
		public float? OneThirdScreenFloatingWindowSizeX { get; set; }
		public float? OneThirdScreenFloatingWindowSizeY { get; set; }
		public bool IsSumAdaptiveModeActive { get; set; }
		public int? SumAdaptiveModeColumn { get; set; }
		public bool IsMSOfficeUsed { get; set; }
		public bool QtBrowser { get; set; }

		public string GetInfo()
		{
			const char hyphen = '-';
			const char colon = ':';
			const char space = ' ';
			const string nullValue = "null";

			var type = GetType();
			var properties = type.GetProperties(MemberBindingFlags.PublicInstanceProperty);
			var propertiesCount = properties.Length;
			var buffer = new StringBuilder();

			_ = buffer.Append(type.Name).Append(colon);
			_ = buffer.AppendLine();

			for (var i = 0; i < propertiesCount; i++)
			{
				var property = properties[i];

				var value = property.GetValue(this);

				_ = buffer.Append(hyphen)
					.Append(space)
					.Append(property.Name)
					.Append(colon)
					.Append(space)
					.Append(value ?? nullValue);

				if (i < propertiesCount - 1)
					_ = buffer.AppendLine();
			}

			return buffer.ToString();
		}
	}
}