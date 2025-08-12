using it.Network.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace it.Popups
{
	public enum TypeCashier
	{
		Deposit,
		Withdrawal
	}

	public class SelectMethod{
		public ICashierMethod Method;
		public TypeCashier Type;
		public string ImageUrl;
		public string Currency;
		public bool IsQR { get; set; }
	}

	public class CashierOperationNavigations : MonoBehaviour
	{
		public System.Action<SelectMethod> OnSelect;

		[SerializeField] private TypeCashier _type;
		[SerializeField] private CashierServisButton _itemPrefab;
		[SerializeField] private ScrollRect _scrollRect;

		private PoolList<CashierServisButton> _pooler;

		private void OnEnable()
		{
			if (_pooler == null)
				_pooler = new PoolList<CashierServisButton>(_itemPrefab.gameObject, _itemPrefab.transform.parent);

			_pooler.HideAll();
			_itemPrefab.gameObject.SetActive(false);

			List<ICashierMethod> methods = _type == TypeCashier.Deposit
			? UserController.Instance.Cashier.CashierMethods.replenishment.ToList<ICashierMethod>()
			: UserController.Instance.Cashier.CashierMethods.payout.ToList<ICashierMethod>();

			methods = methods.OrderBy(x => x.Order).ToList();

			for (int i = 0; i < methods.Count; i++)
			{
				int methodIndex = i;
				if (methods[methodIndex].ServiceInfo != null && methods[methodIndex].ServiceInfo.Length > 0)
				{
					for (int ii = 0; ii < methods[methodIndex].ServiceInfo.Length; ii++)
					{
						AddItemFromService(methods[methodIndex], methods[methodIndex].ServiceInfo[ii].icon, methods[methodIndex].ServiceInfo[ii].currency);
					}
				}
				else
				{
					for (int ii = 0; ii < methods[i].Icons.Count; ii++)
					{
						int iconeIndex = ii;
						var item = _pooler.GetItem();
						item.Set(methods[methodIndex], methods[i].Icons[ii],
						(elem) =>
						{
							SetOperation(new SelectMethod() { Method = elem, Type = _type, ImageUrl = methods[methodIndex].Icons[iconeIndex] });
						});
					}
				}
			}
		}

		private void AddItemFromService(ICashierMethod method, string imageUrl, string currency){

			var item = _pooler.GetItem();
			item.Set(method, imageUrl,
			(elem) =>
			{
				SetOperation(new SelectMethod() { Method = elem, Type = _type, ImageUrl = imageUrl, Currency = currency });
			});
		}

		public void SetOperation(SelectMethod selectMethod)
		{
			OnSelect?.Invoke(selectMethod);
		}

	}
}