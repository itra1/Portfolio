using Game.Scripts.UI.Controllers.Base;
using UnityEngine.Events;

namespace Game.Scripts.UI
{
	public interface IUiNavigator
	{
		/// <summary>
		/// The event of a change in the visibility of the presentrator
		/// </summary>
		UnityEvent<bool, IWindowPresenterController> OnPresenterVisibleChange { get; set; }

		//UniTask<IWindowPresenter> OpenWindow(string windowName);

		T GetController<T>() where T : IWindowPresenterController;

		void CloseAll();
		//bool BackNavigation();
		void ClearStack();
	}
}