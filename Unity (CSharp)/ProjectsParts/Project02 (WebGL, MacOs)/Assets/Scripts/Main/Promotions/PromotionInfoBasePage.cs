using Garilla;

using UnityEngine;

namespace it.UI.Promotions
{
	public class PromotionInfoBasePage : MonoBehaviour
	{
		[SerializeField] private PromotionInfoCategory _category = PromotionInfoCategory.None;
		private const string _keyURLPromotionsInfo = "promotionsInfo";

		public PromotionInfoCategory category => _category;

		public void OpenURLPromotionsInfo()
		{
			LinkManager.OpenUrl(_keyURLPromotionsInfo);
		}
	}
}