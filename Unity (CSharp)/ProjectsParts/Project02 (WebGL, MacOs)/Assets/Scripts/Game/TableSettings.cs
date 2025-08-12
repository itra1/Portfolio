using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ootii.Geometry;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace it.UI
{
	internal class TableSettings : MonoBehaviour
	{
		[SerializeField] private it.UI.Elements.MoveToggle _straggalToggle;
		[SerializeField] private it.UI.Elements.MoveToggle _messagesToggle;
		[SerializeField] private it.UI.Elements.MoveToggle _emojiesToggle;
		[SerializeField] private GamePanel _gamePanel;

		public GamePanel GamePanel { get => _gamePanel; set => _gamePanel = value; }

		private Vector3 _startAncor;
		private RectTransform _rt;
		private CanvasGroup _canv;
		private bool _isVisible;
		private TopMobilePanel _topMobilePanel;

		private void Awake()
		{
			_canv = gameObject.GetOrAddComponent<CanvasGroup>();
			gameObject.GetOrAddComponent<GraphicRaycaster>();
			_canv.alpha = 0;

			_rt = GetComponent<RectTransform>();
			_startAncor = _rt.anchoredPosition;
			_isVisible = false;

#if !UNITY_STANDALONE
			if (_topMobilePanel == null)
				_topMobilePanel = GetComponentInChildren<TopMobilePanel>(true);
#endif
		}

		private void ReadValues()
		{

			_straggalToggle.OnChangeValue.RemoveAllListeners();
			_messagesToggle.OnChangeValue.RemoveAllListeners();
			_emojiesToggle.OnChangeValue.RemoveAllListeners();

			_straggalToggle.IsOn = GamePanel.GameSession.SettingsStraddal;
			_messagesToggle.IsOn = GamePanel.GameSession.SettingsShowMessage;
			_emojiesToggle.IsOn = GamePanel.GameSession.SettingsShowEmojies;

			_straggalToggle.OnChangeValue.AddListener(StraddalToggle);
			_messagesToggle.OnChangeValue.AddListener(MessagesToggle);
			_emojiesToggle.OnChangeValue.AddListener(SmojiesToggle);
		}

		public void StraddalToggle(bool value){
			GamePanel.GameSession.SettingsStraddal = value;
		}

		public void MessagesToggle(bool value)
		{
			GamePanel.GameSession.SettingsShowMessage = value;
		}

		public void SmojiesToggle(bool value)
		{
			GamePanel.GameSession.SettingsShowEmojies = value;
		}

		public void SetVisible(){

			if (_isVisible) return;
			_isVisible = true;
			_canv.alpha = 1;

			ReadValues();

#if !UNITY_STANDALONE
			var tmp = transform.GetComponentInChildren<TopMobilePanel>();
			if (tmp != null)
				tmp.OnBackAction.AddListener(SetHide);
			_topMobilePanel.SwipeListenerAdd();
#endif
			_rt.DOAnchorPos(Vector2.zero, 0.3f);
		}

		public void SetHide()
		{
			if (!_isVisible) return;
			_isVisible = false;

			_rt.DOAnchorPos(_startAncor, 0.3f).OnComplete(() =>
			{
#if !UNITY_STANDALONE
				_topMobilePanel.SwipeListenerRemove();
#endif
				_canv.alpha = 0;
			});
		}

	}
}
