using it.Api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace it.Popups
{
	public class RequestsCashierPage : CashierPage
	{

		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private CashierRequestItem _itemPrefab;

		private PoolList<CashierRequestItem> _poolerItem;
		private List<CashierRequestItem> _recordsList = new List<CashierRequestItem>();

		private void Awake()
		{
			if (_scrollRect != null)
			{
				_scrollRect.onValueChanged.RemoveAllListeners();
				_scrollRect.onValueChanged.AddListener(BaseScrollchange);
			}
		}
		private void OnEnable()
		{
			SpawnTransactions();

		}

		private void BaseScrollchange(Vector2 pos)
		{
			float offset = _scrollRect.content.anchoredPosition.y;

			for (int i = 0; i < _recordsList.Count; i++)
			{
				var itm = _recordsList[i];
				itm.SetRenderer((offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height >= -_scrollRect.viewport.rect.height && offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height <= 0)
				|| (offset + itm.RectRt.anchoredPosition.y >= -_scrollRect.viewport.rect.height && offset + itm.RectRt.anchoredPosition.y <= 0)
				|| (offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height >= 0 && offset + itm.RectRt.anchoredPosition.y <= -_scrollRect.viewport.rect.height)
				);
			}
		}
		private void SpawnTransactions()
		{
			if (_poolerItem == null)
				_poolerItem = new PoolList<CashierRequestItem>(_itemPrefab, _scrollRect.content);

			_poolerItem.HideAll();
			_recordsList.Clear();

			List<string> listSee = new List<string>();

			UserController.Instance.Cashier.GetPaymentRequest(()=> {
				SpawnItems(UserController.Instance.Cashier.RequestTransactions);

				for (int i = 0; i < UserController.Instance.Cashier.RequestTransactions.Count; i++)
					if (UserController.Instance.Cashier.RequestTransactions[i].has_unseen_change)
						listSee.Add(UserController.Instance.Cashier.RequestTransactions[i].id);


				if (listSee.Count > 0)
					UserApi.GetPaymentRequestSeens(listSee, () =>
					{
						//UserController.Instance.GetPaymentRequest();
					},
						(error) =>
						{
						});

			});


		}

		private void SpawnItems(List<UserRequestTransaction> items)
		{

			if (_poolerItem == null)
				_poolerItem = new PoolList<CashierRequestItem>(_itemPrefab, _scrollRect.content);

			_poolerItem.HideAll();

			items = items.OrderByDescending(x => x.CreateAt).ToList();

			int count = 0;
			for (int i = 0; i < items.Count && i < 20; i++)
			{
				var req = items[i];

				if (req.status != "wait") continue;
				count++;

				var item = _poolerItem.GetItem();
				item.SetData(items[i]);
				_recordsList.Add(item);
				item.OnCanceled = () =>
				{
					UserApi.CancelRequest(req.system_id, (res)=> {
						SpawnTransactions();
					});
				};
			}

			RectTransform pRt = _itemPrefab.GetComponent<RectTransform>();
			_scrollRect.content.sizeDelta = new Vector3(_scrollRect.content.sizeDelta.x, pRt.rect.height * count + count * 7);

			BaseScrollchange(Vector2.zero);

		}

	}
}