using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using it.Popups;
using System.Linq;
using DG.Tweening;
using com.ootii.Geometry;

namespace it.Main
{
	public class PopupController : Singleton<PopupController>
	{
		public Image BackBlack { get => _backBlack; set => _backBlack = value; }

		[SerializeField] private Image _backBlack;
		[SerializeField] private RectTransform _backDialogRect;
		[SerializeField] private RectTransform _frontDialogRect;

		//private List<PopupBase> _popups = new List<PopupBase>();
		private List<PopupBase> _openPopups = new List<PopupBase>();

		private void Start()
		{
			if (_backBlack != null)
				_backBlack.gameObject.SetActive(false);
		}


		public void ShowBackGround(bool setShow)
		{
			if (_backBlack == null) return;

			if (_backBlack.gameObject.activeSelf == setShow) return;

			Color black = Color.black;
			black.a = setShow ? 0 : 0.5f;

			if (setShow)
			{
				_backBlack.gameObject.SetActive(true);
				_backBlack.color = black;
				black.a = 0.5f;
				_backBlack.DOColor(black, 0.3f);
			}
			else
			{
				_backBlack.color = black;
				black.a = 0f;
				_backBlack.DOColor(black, 0.3f).OnComplete(() =>
				{
					_backBlack.gameObject.SetActive(false);
				});
			}

		}

		public T ShowPopup<T>(PopupType type, bool force = false) where T : PopupBase
		{
			return ShowPopup(type, force) as T;
		}

		public PopupBase ShowPopup(PopupType type, bool force = false)
		{
#if UNITY_IOS
// «аглушка на ios запрещающа€ открытие кешера
			if (type == PopupType.Cashier && !AppConfig.ActiveCashier) return null;

#endif


			//if (_popups.Count == 0)
			//	FindPopups();
			if (_openPopups.Exists(x => x.Type == type))
			{
				return _openPopups.Find(x => x.Type == type);
			}
			it.Logger.Log("Open popup type " + type.ToString());

			//PopupBase newPopupResource = Resources.Load<PopupBase>($"Prefabs/UI/Popups/{type.ToString()}Popup");
			PopupBase newPopupResource = ((GameObject)Garilla.ResourceManager.GetResource<GameObject>($"Prefabs/UI/Popups/{type.ToString()}Popup")).GetComponent<PopupBase>();

			newPopupResource.gameObject.SetActive(false);
			GameObject newPopupObject = Instantiate(newPopupResource.gameObject, transform);
			PopupBase popup = newPopupObject.GetComponent<PopupBase>();
			//PopupBase popup = _popups.Find(x => x.Type == type);
			if (popup == null) return null;


			if (_openPopups.Count == 0 && popup.DarkBlack)
				ShowBackGround(true);
			if (!_openPopups.Contains(popup))
				_openPopups.Add(popup);
			popup.OnClose -= ClosePanel;
			popup.OnClose += ClosePanel;

			popup.Show(force);
			popup.Focus();

			PopupBase frontPopup = _frontDialogRect.GetComponentInChildren<PopupBase>();
			if (frontPopup == null || frontPopup.Priority <= popup.Priority)
			{
				foreach (var p in _openPopups)
					p.transform.SetParent(_backDialogRect);

				popup.transform.SetParent(_frontDialogRect);
			}
			else
			{
				popup.transform.SetParent(_backDialogRect);

				if (_backDialogRect.childCount > 0)
				{
					bool positing = false;
					for (int i = _backDialogRect.childCount - 1; i >= 0; i--)
					{
						var pb = _backDialogRect.GetChild(i).GetComponent<PopupBase>();
						if (pb.Priority <= 0)
						{
							popup.transform.SetSiblingIndex(i + 1);
							positing = true;
						}
					}
					if (!positing)
						popup.transform.SetAsFirstSibling();

				}

			}
#if UNITY_IOS
			popup.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
			popup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
#endif
			//var pCanv = popup.GetOrAddComponent<Canvas>();
			//var pGraph = popup.GetOrAddComponent<GraphicRaycaster>();
			//pCanv.overrideSorting = true;
			//pCanv.sortingOrder = 26000;

			return popup;
		}
		public void ClosePopup(PopupType type)
		{
			PopupBase popup = _openPopups.Find(x => x.Type == type);
			if (popup == null) return;

			if (!popup.gameObject.activeSelf) return;

			popup.Hide();
		}

		public void ClosePanel(PopupBase popup)
		{
			popup.OnClose -= ClosePanel;
			_openPopups.Remove(popup);

			if (_openPopups.Count > 0)
				_openPopups[_openPopups.Count - 1].transform.SetParent(_frontDialogRect);

			it.Logger.Log("Close panel");

			for (int i = 0; i < _openPopups.Count; i++)
				it.Logger.Log("In close panel " + popup.Type.ToString());


			bool visibleBlack = _openPopups.Exists(x => x.DarkBlack);

			if (!visibleBlack || _openPopups.Count == 0)
				ShowBackGround(false);
			Destroy(popup.gameObject);
		}

		public PopupBase HidePopUps(PopupType type)
		{
			it.Logger.Log("Show panel " + type.ToString());

			PopupBase popup = _openPopups.Find(x => x.Type == type);
			if (popup == null) return null;

			if (_openPopups.Count == 0 && popup.DarkBlack)
				ShowBackGround(true);
			if (!_openPopups.Contains(popup))
				_openPopups.Add(popup);
			popup.OnClose -= ClosePanel;
			popup.OnClose += ClosePanel;

			popup.Hide();
			return popup;
		}

	}
}