using System;

namespace Game.Scripts.UI.Attributes
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
		/// <param name="type">WindowPresenterType</param>
		/// <param name="presenterName">WindowPresenterNames</param>
		public UiControllerAttribute(string type, string presenterName)
		{
			PresenterName = presenterName;
			PresenterType = type;
		}
	}
}
