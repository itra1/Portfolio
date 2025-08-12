using Settings.SymbolOptions.Base;

namespace Engine.Editor.Options
{
	internal class LogEditorOnnly : IToggleDefine
	{
		public string Symbol => "LOG_EDITOR_ONLY";

		public string Description => "Лог только в редакторе";

		public void AfterDisable()
		{
		}

		public void AfterEnable()
		{
		}
	}
}
