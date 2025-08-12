using Core.Engine.App.Base.Attributes.Defines;

namespace Core.Editor.SymbolOptions
{
	public class LogEditorOnly :IToggleDefine
	{
		public string Symbol => "LOG_EDITOR_ONLY";

		public string Description => "Выводить лог только в редакторе";

		public void AfterDisable()
		{
		}

		public void AfterEnable()
		{
		}
	}
}
