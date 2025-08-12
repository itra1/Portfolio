using System.Runtime.CompilerServices;

namespace Core.Elements.ScreenModes
{
	public static class ScreenModeConverter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScreenMode Convert(string alias)
		{
			return alias switch
			{
				ScreenModeName.Status => ScreenMode.Status,
				ScreenModeName.Presentation => ScreenMode.Presentation,
				ScreenModeName.Desktop => ScreenMode.Desktop,
				_ => ScreenMode.None
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Convert(ScreenMode mode)
		{
			return mode switch
			{
				ScreenMode.Status => ScreenModeName.Status,
				ScreenMode.Presentation => ScreenModeName.Presentation,
				ScreenMode.Desktop => ScreenModeName.Desktop,
				_ => string.Empty
			};
		}
	}
}