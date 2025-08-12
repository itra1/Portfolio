using Platforms.Iap.Adapters.Unity.Base;
using UnityEngine.Events;

namespace Platforms.Iap.Adapters
{
	/// <summary>
	/// Адапттер стора
	/// </summary>
	public interface ITDPurchaseAdapter
	{
		/// <summary>
		/// Результат инициализации
		/// bool - Итог инициализации
		/// </summary>
		UnityAction<bool> OnInitializeResult { get; set; }

		/// <summary>
		/// Результат покупки
		/// TDPurchaseResult - результат покупки
		/// </summary>
		UnityAction<TDPurchaseResult> OnPurchaseResult { get; set; }

		/// <summary>
		/// Результат восстановления покупок
		/// </summary>
		UnityAction<bool> OnRestoreResult { get; set; }

		/// <summary>
		/// Адаптер инициализирован
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Покупка
		/// </summary>
		/// <param name="productId">Внетренний идентификаторт продукта</param>
		/// <returns></returns>
		bool Purchase(string productId);
		float GetPrice(string productId);
		string GetPriceString(string productId);
		void RestorePurchases();
	}
}
