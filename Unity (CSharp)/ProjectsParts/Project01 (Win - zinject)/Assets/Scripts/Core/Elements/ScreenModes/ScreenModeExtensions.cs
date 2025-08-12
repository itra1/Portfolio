using System.Runtime.CompilerServices;

namespace Core.Elements.ScreenModes
{
	public static class ScreenModeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this ScreenMode mode) => ScreenModeConverter.Convert(mode);
	}
}