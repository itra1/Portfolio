using System;

namespace Game.Providers.Ui.Attributes
{
	/// <summary>
	/// Аттрибут для контроллеро врезентеров
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class UiControllerAttribute : Attribute, IUiControllerAttribute
	{
		public string PresenterName { get; private set; }
		public string PresenterType { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="presenterType">WindowPresenterType</param>
		/// <param name="presenterName">WindowPresenterNames</param>
		public UiControllerAttribute(string presenterType, string presenterName)
		{
			PresenterName = presenterName;
			PresenterType = presenterType;
		}
	}
}
