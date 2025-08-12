using Core.Engine.App.Base.Attributes.Defines;

namespace Core.Editor.SymbolOptions
{
	public class UniTaskDoTweenSupport :IToggleDefine
	{
		public string Symbol => "UNITASK_DOTWEEN_SUPPORT";

		public string Description => "Включение поддержики Dotweet Unitask";

		public void AfterDisable()
		{
		}

		public void AfterEnable()
		{
		}
	}
}
