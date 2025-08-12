using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using it.Api;
using DG.Tweening;

namespace it.UI.Settings
{
	public class BalanceHistorySettingsPage : MonoBehaviour
	{
		[SerializeField] private ScrollRect _scroll;
		[SerializeField] private BalanceHistoryRecord _recordPrefab;
		[SerializeField] private Image _dateTimeSorterArrow;
		[SerializeField] private Image _dateTimeSorterDescArrow;
		[SerializeField] private Image _amountSorterArrow;
		[SerializeField] private Image _amountSorterDescArrow;
		[SerializeField] private Image _categorySorterArrow;
		[SerializeField] private Image _categorySorterDescArrow;
		[SerializeField] private Color _sorterDisactiveColor;
		[SerializeField] private Color _sorterActiveColor;

		private SotrerType _sorter;
		private Tween _waitRequest;

		[System.Flags]
		private enum SotrerType
		{
			None = 0,
			Datetime = 1,
			DatetimeDesc = 2,
			Amount = 4,
			AmountDesc = 8,
			Category = 16,
			CategoryDesc = 32
		}

		private PoolList<BalanceHistoryRecord> _pooler;

		private void InitPooler()
		{
			if (_pooler == null)
				_pooler = new PoolList<BalanceHistoryRecord>(_recordPrefab, _scroll.content);

			_recordPrefab.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			_sorter = SotrerType.None;
			ConfirmSorter();

			InitPooler();
			GetRecords(true);
		}

		private void OnDisable()
		{
			if (_waitRequest != null)
				_waitRequest.Kill();
		}

		private void GetRecords(bool force = false)
		{
			if (_waitRequest != null)
				_waitRequest.Kill();

			if (force)
				RequestRecords();
			else
			{
				_waitRequest = DOVirtual.DelayedCall(1, () =>
				{
					RequestRecords();
				});
			}

		}

		private void RequestRecords()
		{

			UserApi.GetPaymentTransactions(GetOptions(), (responce) =>
			{
				SpawnRecords(responce.data);

			}, (error) =>
			{
			});
		}

		private void SpawnRecords(List<UserWalletTransaction> transactions)
		{
			InitPooler();

			_pooler.HideAll();

			for (int i = 0; i < transactions.Count; i++)
			{
				var item = _pooler.GetItem();
				item.SetData(transactions[i]);
			}

			RectTransform rtP = _recordPrefab.GetComponent<RectTransform>();

			_scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, transactions.Count * rtP.rect.height);

		}

		private string GetOptions()
		{
			string result = "without_pagination=1";

			if (_sorter == SotrerType.Datetime)
				result += "&order_by=created_at&order_direction=asc";
			if (_sorter == SotrerType.DatetimeDesc)
				result += "&order_by=created_at&order_direction=desc";

			if (_sorter == SotrerType.Amount)
				result += "&order_by=amount&order_direction=asc";
			if (_sorter == SotrerType.AmountDesc)
				result += "&order_by=amount&order_direction=desc";

			if (_sorter == SotrerType.Category)
				result += "&order_by=user_wallet_transaction_type.slug&order_direction=asc";
			if (_sorter == SotrerType.CategoryDesc)
				result += "&order_by=user_wallet_transaction_type.slug&order_direction=desc";

			if (_sorter == SotrerType.None)
				result += "&order_by=created_at&order_direction=desc";

			return result;
		}

		public void SorterDatetimeTouch()
		{
			if (_sorter == SotrerType.DatetimeDesc)
			{
				_sorter = SotrerType.None;
				ConfirmSorter();
				GetRecords();
				return;
			}else	if (_sorter == SotrerType.Datetime)
			{
				_sorter = SotrerType.DatetimeDesc;
				ConfirmSorter();
				GetRecords();
				return;
			}else{
				_sorter = SotrerType.Datetime;
				ConfirmSorter();
				GetRecords();
			}
		}

		public void SorterAmountTouch()
		{
			if (_sorter == SotrerType.AmountDesc)
			{
				_sorter = SotrerType.None;
				ConfirmSorter();
				GetRecords();
				return;
			}else	if (_sorter == SotrerType.Amount)
			{
				_sorter = SotrerType.AmountDesc;
				ConfirmSorter();
				GetRecords();
				return;
			}
			else
			{
				_sorter = SotrerType.Amount;
				ConfirmSorter();
				GetRecords();
				return;
			}
		}
		public void SorterCategoryTouch()
		{
			if (_sorter == SotrerType.CategoryDesc)
			{
				_sorter = SotrerType.None;
				ConfirmSorter();
				GetRecords();
				return;
			}else if (_sorter == SotrerType.Category)
			{
				_sorter = SotrerType.CategoryDesc;
				ConfirmSorter();
				GetRecords();
				return;
			}else
			{
				_sorter = SotrerType.Category;
				ConfirmSorter();
				GetRecords();
			}
		}
		private void ConfirmSorter()
		{
			_dateTimeSorterArrow.color = _sorter == SotrerType.Datetime ? _sorterActiveColor : _sorterDisactiveColor;
			_dateTimeSorterDescArrow.color = _sorter == SotrerType.DatetimeDesc ? _sorterActiveColor : _sorterDisactiveColor;
			_amountSorterArrow.color = _sorter == SotrerType.Amount ? _sorterActiveColor : _sorterDisactiveColor;
			_amountSorterDescArrow.color = _sorter == SotrerType.AmountDesc ? _sorterActiveColor : _sorterDisactiveColor;
			_categorySorterArrow.color = _sorter == SotrerType.Category ? _sorterActiveColor : _sorterDisactiveColor;
			_categorySorterDescArrow.color = _sorter == SotrerType.CategoryDesc ? _sorterActiveColor : _sorterDisactiveColor;
		}


		//public void SorterDatetimeTouch()
		//{
		//	if ((_sorter & SotrerType.DatetimeDesc) != 0)
		//	{
		//		_sorter ^= SotrerType.DatetimeDesc;
		//		ConfirmSorter();
		//		return;
		//	}
		//	if ((_sorter & SotrerType.Datetime) == 0)
		//	{
		//		_sorter |= SotrerType.Datetime;
		//		ConfirmSorter();
		//		return;
		//	}
		//	if ((_sorter & SotrerType.Datetime) != 0)
		//	{
		//		_sorter ^= SotrerType.Datetime;
		//		_sorter |= SotrerType.DatetimeDesc;
		//		ConfirmSorter();
		//		return;
		//	}
		//}

		//public void SorterAmountTouch()
		//{
		//	if ((_sorter & SotrerType.AmountDesc) != 0)
		//	{
		//		_sorter ^= SotrerType.AmountDesc;
		//		ConfirmSorter();
		//		return;
		//	}
		//	if ((_sorter & SotrerType.Amount) == 0)
		//	{
		//		_sorter |= SotrerType.Amount;
		//		ConfirmSorter();
		//		return;
		//	}
		//	if ((_sorter & SotrerType.Amount) != 0)
		//	{
		//		_sorter ^= SotrerType.Amount;
		//		_sorter |= SotrerType.AmountDesc;
		//		ConfirmSorter();
		//		return;
		//	}
		//}
		//public void SorterCategoryTouch()
		//{
		//	if ((_sorter & SotrerType.CategoryDesc) != 0)
		//	{
		//		_sorter ^= SotrerType.CategoryDesc;
		//		ConfirmSorter();
		//		return;
		//	}
		//	if ((_sorter & SotrerType.Category) == 0)
		//	{
		//		_sorter |= SotrerType.Category;
		//		ConfirmSorter();
		//		return;
		//	}
		//	if ((_sorter & SotrerType.Category) != 0)
		//	{
		//		_sorter ^= SotrerType.Category;
		//		_sorter |= SotrerType.CategoryDesc;
		//		ConfirmSorter();
		//		return;
		//	}
		//}


	}
}