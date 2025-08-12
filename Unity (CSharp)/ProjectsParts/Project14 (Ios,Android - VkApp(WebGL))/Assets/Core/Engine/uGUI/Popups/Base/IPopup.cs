using Cysharp.Threading.Tasks;

using UnityEngine.Events;

namespace Core.Engine.uGUI.Popups {
	public interface IPopup {
		/// <summary>
		/// Показать попап
		/// </summary>
		UniTask Show();

		/// <summary>
		/// Скрыть попап
		/// </summary>
		UniTask Hide();

		/// <summary>
		/// Подписаться на событие начала показа
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		IPopup OnShow(UnityAction callback);
		/// <summary>
		/// Подписаться на событие полного показа
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		IPopup OnShowComplete(UnityAction callback);
		/// <summary>
		/// Подписаться на событие начала показа
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		IPopup OnHide(UnityAction callback);
		/// <summary>
		/// Подписаться на событие окончания показа
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		IPopup OnHideComplete(UnityAction callback);
	}
}
