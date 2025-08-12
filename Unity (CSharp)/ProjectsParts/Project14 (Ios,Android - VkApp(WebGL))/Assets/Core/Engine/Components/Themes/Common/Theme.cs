using UnityEngine;

namespace Core.Engine.Components.Themes.Common {
	public abstract class Theme : MonoBehaviour, IThemeItem {
		public abstract string UUID { get; }

		public abstract bool IsDefault { get; }

		public abstract Color Ball { get; }

		public abstract bool Confirm(IThemeComponents themeComponents);
	}
}
