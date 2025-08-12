using Game.Common.Attributes.Defines;

namespace Core.Editor.SymbolOptions {
	public class UniTaskDoTweenSupport :IToggleDefine {
		public string Symbol => "UNITASK_DOTWEEN_SUPPORT";

		public string Description => "Dotweet UniTask";

		public void AfterDisable() {
		}

		public void AfterEnable() {
		}
	}
}
