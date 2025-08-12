using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Game.Scripts.UI.Controllers.Base
{
	public interface IWindowPresenterControllerBase
	{
		/// <summary>
		/// Event of a change in the visibility of the presenter
		/// 1 - the need to add to the stack or reset stack and add as the first
		/// 2 - set as visible
		/// 3 - presenter
		/// </summary>

		UnityEvent<IWindowPresenterController> OnPresenterVisibleChange { get; set; }

		bool IsOpen { get; }

		/// <summary>
		/// Open the presenter
		/// </summary>
		/// <param name="clearNavigationStack">Pre -clean the stack</param>
		/// <param name="addInNavigationStack">Add the element to the stack</param>
		/// <returns></returns>
		UniTask<bool> Open();
		UniTask<bool> Close();
		//UniTask<bool> BackStackNavigation();
	}
}