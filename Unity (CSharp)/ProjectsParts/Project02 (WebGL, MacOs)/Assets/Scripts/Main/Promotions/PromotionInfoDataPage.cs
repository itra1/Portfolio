using System.Collections.Generic;

using UnityEngine;

namespace it.UI.Promotions
{
	public class PromotionInfoDataPage : PromotionInfoBasePage
	{
		[SerializeField] private Transform _infoDataParent;
		[SerializeField] private PromotionInfoDataComponent _infoDataPrefab;

		private Dictionary<string, PromotionInfoDataComponent> _infoDataComponents = new Dictionary<string, PromotionInfoDataComponent>();

		public void SetData(List<PromotionInfoDataComponent.InfoData> infoData)
		{
			foreach (var data in infoData)
			{
				if (_infoDataComponents.ContainsKey(data.type))
				{
					_infoDataComponents[data.type].UpdateProgress(data.progress);
				}
				else
				{
					var component = Instantiate(_infoDataPrefab, _infoDataParent);
					component.SetData(data);
					_infoDataComponents.Add(component.typeData, component);
				}
			}
		}

		public void Clear()
		{
			_infoDataComponents.Clear();

			foreach (Transform child in _infoDataParent)
			{
				Destroy(child.gameObject);
			}
		}
	}
}