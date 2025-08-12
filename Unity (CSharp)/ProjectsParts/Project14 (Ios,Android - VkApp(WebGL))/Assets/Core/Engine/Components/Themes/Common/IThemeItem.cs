
using UnityEngine;

namespace Core.Engine.Components.Themes.Common {
	public interface IThemeItem {
		string UUID { get; }
		bool IsDefault { get; }
		bool Confirm(IThemeComponents themeComponents);
		Color Ball { get; }
	}
}
