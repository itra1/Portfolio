using Settings.SymbolOptions.Base;

namespace Engine.Editor.Options
{
	public class DisableTutorial : IToggleDefine
	{
		public string Symbol => "DISABLE_TUTORIAL";

		public string Description => "Отключить туториал";

		public void AfterDisable()
		{
		}

		public void AfterEnable()
		{
		}
	}
}
