using Core.Engine.Components.Shop;

using UnityEngine;

namespace Core.Engine.uGUI.Elements
{
	public abstract class ShopProductPanel : MonoBehaviour
	{
		private RectTransform _rt;
		protected IShopProduct _product;

		public RectTransform RT => _rt ??= GetComponent<RectTransform>();

		public void Set(IShopProduct product)
		{
			_product?.UnSubscribeChange(Confirm);
			_product = product;
			_product.SubscribeChange(Confirm);

			Confirm();
		}
		private void OnDisable()
		{
			_product?.UnSubscribeChange(Confirm);
		}

		protected abstract void Confirm();
		public void Buy()
		{
			if (_product.Buy())
			{
				Confirm();
			}
		}

	}
}
