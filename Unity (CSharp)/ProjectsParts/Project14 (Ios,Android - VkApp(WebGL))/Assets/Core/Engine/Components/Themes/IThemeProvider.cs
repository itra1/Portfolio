using Core.Engine.Components.Themes.Common;
using System.Collections.Generic;

namespace Core.Engine.Components.Themes {
	public interface IThemeProvider {
		List<IThemeItem> GetList { get; }

		IThemeItem ActiveTheme { get; }
		void SetTheme(IThemeItem themeItem);
	}
}
