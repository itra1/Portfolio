using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Main
{
	public class UserPanel : MonoBehaviour
	{
		[SerializeField] private GameObject _guestPanel;
		[SerializeField] private GameObject _playerPanel;

		private RectTransform _guestRect;
		private RectTransform _playeRect;
		private float _timeAnimate = 0.5f;
		private void OnEnable()
		{
			_guestPanel.gameObject.SetActive(true);
			_playerPanel.gameObject.SetActive(false);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, OnAuthorization);
			_guestRect = _guestPanel.GetComponent<RectTransform>();
			_playeRect = _playerPanel.GetComponent<RectTransform>();
			ConfirmPanel();

			_playeRect.anchoredPosition = new Vector2(_playeRect.rect.width, _playeRect.anchoredPosition.y);
		}

		private void OnAuthorization(com.ootii.Messages.IMessage handler)
		{
			ConfirmPanel();
		}

		private void ConfirmPanel()
		{
			if (_playeRect == null)
				_playeRect = _playerPanel.GetComponent<RectTransform>();
			if (_guestRect == null)
				_guestRect = _guestPanel.GetComponent<RectTransform>();

			if (UserController.IsLogin)
			{
				_playeRect.gameObject.SetActive(true);
				_playeRect.DOAnchorPos(new Vector2(0, _playeRect.anchoredPosition.y), _timeAnimate).OnComplete(() =>
				{
					_guestPanel.gameObject.SetActive(false);
				});
				_playerPanel.GetComponent<PlayerPanel>().Init();
			}
			else
			{
				_guestRect.gameObject.SetActive(true);
				_playeRect.DOAnchorPos(new Vector2(_guestRect.rect.width, _playeRect.anchoredPosition.y), _timeAnimate).OnComplete(() =>
				{
					_playeRect.gameObject.SetActive(false);
				});
			}
		}

	}
}