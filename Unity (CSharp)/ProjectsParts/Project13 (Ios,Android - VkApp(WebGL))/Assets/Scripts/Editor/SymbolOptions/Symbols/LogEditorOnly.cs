using Game.Common.Attributes.Defines;

namespace Game.Editor.SymbolOptions {
	public class LogEditorOnly :IToggleDefine {
		public string Symbol => "LOG_EDITOR_ONLY";

		public string Description => "Log Editor only";

		public void AfterDisable() {
		}

		public void AfterEnable() {
		}
	}
}
