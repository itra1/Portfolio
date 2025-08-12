using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using it.Api;
using UnityEngine.UI;
using DG.Tweening;
 

namespace it.Popups
{
	public class TransfersCashierPage : CashierPage
	{
		[SerializeField] protected TMP_InputField _amountField;
		[SerializeField] protected TMP_InputField _accountName;
		[SerializeField] protected it.UI.Avatar _myAvatar;
		[SerializeField] protected TextMeshProUGUI _myName;
		[SerializeField] protected it.UI.Avatar _targetAvatar;
		[SerializeField] protected TextMeshProUGUI _targetName;
		[SerializeField] protected it.UI.Elements.GraphicButtonUI _applyButton;
		[SerializeField] protected RectTransform _contextRect;
		[SerializeField] protected ScrollRect _scroll;
		[SerializeField] protected GameObject _itemContect;

		protected FindUser _findUser;
		protected PoolList<RectTransform> _pooler;
		protected Tween _timerToReuest;

		protected virtual void Awake()
		{
			_myAvatar.SetDefaultAvatar();
			_targetAvatar.SetDefaultAvatar();
			SubscribeChangeField();
		}

		protected void SubscribeChangeField()
		{
			_accountName.onValueChanged.RemoveAllListeners();
			_accountName.onValueChanged.AddListener((val) =>
			{
				ClearError();
				if (_timerToReuest != null && _timerToReuest.IsActive())
					_timerToReuest.Kill();

				_timerToReuest = DOVirtual.DelayedCall(0.5f, () =>
				{
					if (val.Length < 3)
					{
						_contextRect.gameObject.SetActive(false);
						return;
					}

					FindUser(val);
				});

			});

			_amountField.onValueChanged.RemoveAllListeners();
			_amountField.onValueChanged.AddListener((val) =>
			{
				AmountChange(val);
			});
		}

		protected virtual void AmountChange(string val){

		}

		protected virtual void ClearError(){	}

		protected virtual void OnEnable()
		{
			_myAvatar.SetAvatar(UserController.User.AvatarUrl);
			_myName.text = UserController.User.nickname;
			//_applyButton.interactable = false;
			ClearError();
			ClearUser();
			_contextRect.gameObject.SetActive(false);
		}

		protected void SpawnLiad(List<FindUser> userList)
		{

			if (_pooler == null)
				_pooler = new PoolList<RectTransform>(_itemContect, _scroll.content);

			_pooler.HideAll();

			if (userList.Count <= 0)
			{
				_contextRect.gameObject.SetActive(false);
				return;
			}

			_contextRect.gameObject.SetActive(true);

			for (int i = 0; i < userList.Count; i++)
			{
				if (userList[i].id > 0 && userList[i].id == UserController.User.id) continue;


				//count++;
				int index = i;
				var itm = _pooler.GetItem();

				var lbl = itm.GetComponentInChildren<TextMeshProUGUI>();
				lbl.text = userList[i].nickname;

				var btn = itm.GetComponent<it.UI.Elements.GraphicButtonUI>();
				btn.OnClick.RemoveAllListeners();
				btn.OnClick.AddListener(() =>
				{
					ConfirmUser(userList[index]);
				});

			}

			_contextRect.sizeDelta = new Vector2(_contextRect.sizeDelta.x, Mathf.Min(5, userList.Count) * _itemContect.GetComponent<RectTransform>().rect.height + -_scroll.viewport.sizeDelta.y);

		}

		private void ConfirmUser(FindUser user)
		{
			_contextRect.gameObject.SetActive(false);
			_findUser = user;
			if (!string.IsNullOrEmpty(_findUser.avatar_url))
				_targetAvatar.SetAvatar(user.avatar_url);
			else
				_targetAvatar.SetDefaultAvatar();
			_accountName.onValueChanged.RemoveAllListeners();
			_accountName.text = _findUser.nickname;
			_targetName.text = _findUser.nickname;
			SubscribeChangeField();
			//_applyButton.interactable = true;
		}
		private void ClearUser()
		{
			_findUser = null;
			_targetAvatar.SetDefaultAvatar();
			_accountName.text = "";
			_targetName.text = "-";
			//_applyButton.interactable = false;
		}

		protected virtual void FindUser(string val){

		}

		public virtual void TransferButtonTouch(){

		}


	}
}