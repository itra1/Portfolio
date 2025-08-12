using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using it.Network.Rest;
using System.Linq;
using System.Collections.Generic;
using it.Api;
using UnityEngine.Profiling;

namespace it.Popups
{
	public interface ITransactionItem
	{
		RectTransform RectRt { get; }
		void SetRenderer(bool isVisible);
	}


	public class TransactionsCashierPage : CashierPage
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private CashierTransactionItem _transactionPrefab;
		[SerializeField] private CashierRequestItem _requestPrefab;

		private PoolList<CashierTransactionItem> _transactionsPooler;
		private PoolList<CashierRequestItem> _requestPooler;
		private List<ITransactionItem> _transactionsList = new List<ITransactionItem>();


		private List<UserRequestTransaction> _reauestReady = new List<UserRequestTransaction>();
		private List<UserWalletTransaction> _transactionReady = new List<UserWalletTransaction>();
		private bool _isRequests;
		private bool _isTransactions;

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
			_transactionPrefab.gameObject.SetActive(false);
			_requestPrefab.gameObject.SetActive(false);
			SpawnTransactions();
		}

		private void BaseScrollchange(Vector2 pos)
		{
			float offset = _scrollRect.content.anchoredPosition.y;

			for (int i = 0; i < _transactionsList.Count; i++)
			{
				var itm = _transactionsList[i];
				itm.SetRenderer((offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height >= -_scrollRect.viewport.rect.height && offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height <= 0)
				|| (offset + itm.RectRt.anchoredPosition.y >= -_scrollRect.viewport.rect.height && offset + itm.RectRt.anchoredPosition.y <= 0)
				|| (offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height >= 0 && offset + itm.RectRt.anchoredPosition.y <= -_scrollRect.viewport.rect.height)
				);
			}
		}

		private void SpawnTransactions()
		{
			_isRequests = false;
			_isTransactions = false;

			if (_transactionsPooler == null)
				_transactionsPooler = new PoolList<CashierTransactionItem>(_transactionPrefab, _scrollRect.content);
			if (_requestPooler == null)
				_requestPooler = new PoolList<CashierRequestItem>(_requestPrefab, _scrollRect.content);

			_transactionsPooler.HideAll();
			_requestPooler.HideAll();

			_reauestReady.Clear();
			_transactionReady.Clear();

			// "filters=user_wallet_transaction_type.slug,in,replenishment,payout"
			UserApi.GetPaymentTransactions("filters=user_wallet_transaction_type.slug,in,payout,transfer_out,transfer_in,replenishment", (result) =>
			{

				//var records = result.data.OrderByDescending(x => x.id).ToList();
				_isTransactions = true;
				_transactionReady = new List<UserWalletTransaction>(result.data);
				CheckAndSpawn();

			},
		 (error) =>
		 {
		 });


			UserController.Instance.Cashier.GetPaymentRequest(() =>
			{
				_isRequests = true;
				_reauestReady = new List<UserRequestTransaction>(UserController.Instance.Cashier.RequestTransactions);
				CheckAndSpawn();
			});

		}

		private void CheckAndSpawn()
		{
			if (!_isRequests || !_isTransactions) return;

			List<ITransactionType> spawnList = new List<ITransactionType>();
			spawnList.AddRange(_reauestReady);
			spawnList.AddRange(_transactionReady);
			SpawnItems(spawnList);
		}

		private void SpawnItems(List<ITransactionType> userWallerTransactions)
		{

			userWallerTransactions = userWallerTransactions.OrderByDescending(x => x.CreateAt).ToList();

			int count = 0;

			for (int i = 0; i < userWallerTransactions.Count && i < 20; i++)
			{
				var resItem = userWallerTransactions[i];

				if(resItem.TransactionType == "transaction")
				{
					var source = userWallerTransactions[i] as UserWalletTransaction;
					// payout,transfer_out,transfer_in
					if (source.user_wallet_transaction_type.slug != "payout"
					&& source.user_wallet_transaction_type.slug != "transfer_out"
					&& source.user_wallet_transaction_type.slug != "transfer_in"
					&& source.user_wallet_transaction_type.slug != "replenishment") continue;

					if(source.wallet_transactionable != null &&
					_reauestReady.Exists(x=>x.id == source.wallet_transactionable.id))
						continue;

					count++;

					var item = _transactionsPooler.GetItem();
					item.SetData(source);
					_transactionsList.Add(item);
				}else
				{
					var source = userWallerTransactions[i] as UserRequestTransaction;
					if (source.status == "wait") continue;
					count++;
					var item = _requestPooler.GetItem();
					item.SetData(userWallerTransactions[i] as UserRequestTransaction);
					_transactionsList.Add(item);
				}

			}

			RectTransform pRt = _transactionPrefab.GetComponent<RectTransform>();
			_scrollRect.content.sizeDelta = new Vector3(_scrollRect.content.sizeDelta.x, pRt.rect.height * count + count * 7);

			BaseScrollchange(Vector2.zero);
		}

	}
}