using com.ootii.Geometry;
using it.UI;
using it.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using it.Network.Rest;
using it.Settings;
using UnityEngine.Events;
using System;
using System.Data.SqlTypes;
using it.Main;

namespace Garilla.Games
{
	public class MobileGameTopPanel : MonoBehaviour, ISwipe
	{
		public UnityEngine.Events.UnityAction OnCloseButton;
		public UnityEngine.Events.UnityAction OnObserveButton;
		public UnityEngine.Events.UnityAction OnHomeButton;
		public UnityEngine.Events.UnityAction OnSettingsButton;
		public UnityEngine.Events.UnityAction OnThemeButton;
		public UnityEngine.Events.UnityAction OnWCLButton;
		public UnityEngine.Events.UnityAction OnAddButton;
		public UnityEngine.Events.UnityAction<GamePanel> OnTableClick;

		[SerializeField] private CanvasGroup _cg;
		[SerializeField] private GraphicButtonUI _closeButton;
		[SerializeField] private GraphicButtonUI _observersButton;
		[SerializeField] private GraphicButtonUI _homeButton;
		[SerializeField] private GraphicButtonUI _settingsButton;
		[SerializeField] private RectTransform _parentTableList;
		[SerializeField] private ScrollRect _scrollItems;
		[SerializeField] private GraphicButtonUI _addTableButton;
		[SerializeField] private GraphicButtonUI _tableButton;

		private PoolList<MobileTablePictogramm> _tableIconsPooler;
		private List<MobileTablePictogramm> _tablePanels = new List<MobileTablePictogramm>();
		private bool _visibleTables = false;
		private RectTransform _rt;

		public bool VisibleTables
		{
			get
			{
				return _visibleTables;
			}
			set
			{
				if (_visibleTables == value) return;
				_visibleTables = value;

				_closeButton.gameObject.SetActive(_visibleTables);
				_observersButton.gameObject.SetActive(_visibleTables);
				_homeButton.gameObject.SetActive(_visibleTables);
				//_settingsButton.gameObject.SetActive(_visibleTables);

				if (_rt == null)
					_rt = GetComponent<RectTransform>();

				if (_visibleTables)
				{
					_rt.DOAnchorPos(Vector2.zero, 0.3f);
					SwipeAddListener();
				}
				else
				{
					_tablePanels.ForEach(x => x.IsSelect = false);
					_rt.DOAnchorPos(new Vector2(0, -_rt.rect.height), 0.3f);
					SwipeRemoveListener();
				}
			}
		}

		public bool Visible
		{
			get => _cg.blocksRaycasts;
			set
			{
				if (value)
				{
					_cg.blocksRaycasts = true;
					DOTween.To(() => _cg.alpha, (x) => _cg.alpha = x, 1, 0.3f);
				}
				else
				{
					_cg.blocksRaycasts = false;
					DOTween.To(() => _cg.alpha, (x) => _cg.alpha = x, 0, 0.3f);
				}
			}
		}

		public SwipeToClose SwipeType => SwipeToClose.Right | SwipeToClose.Left;

		public UnityAction<SwipeToClose> OnSwipeEvent => (swipe) =>
		{
			if (_tablePanels.Count <= 1) return;

			if ((swipe & SwipeToClose.Right) != 0)
				BeforeGame();
			if ((swipe & SwipeToClose.Left) != 0)
				NextGame();
		};

		public void SwipeAddListener()
		{
			SwipeManager.AddListener(this);
		}

		public void SwipeRemoveListener()
		{
			SwipeManager.RemoveListener(this);
		}

		private void Awake()
		{
			var cmp = _tableButton.gameObject.AddComponent<MobileTablePictogramm>();

			if (_tableIconsPooler == null)
				_tableIconsPooler = new PoolList<MobileTablePictogramm>(cmp.gameObject, _scrollItems.content);
			ResizeScrollContent();
			cmp.gameObject.SetActive(false);
		}

		private void Start()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.PageSelect, (h) =>
			{
				Visible = MobileTablesUIManager.Instance.CurrentPage == "Main" && SinglePageController.Instance.Pages.Count <= 0;
			});
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SinglePageChnage, (h) =>
			{
				Visible = MobileTablesUIManager.Instance.CurrentPage == "Main" && SinglePageController.Instance.Pages.Count <= 0;
			});
		}

		public void AddTable(GamePanel newGamePanel)
		{
			var panel = newGamePanel;
			panel.gameObject.name = panel.Table.id.ToString();
			var itm = _tableIconsPooler.GetItem();
			itm.SetGamePanel(panel);
			_tablePanels.Add(itm);
			_addTableButton.gameObject.SetActive(_tablePanels.Count <= 3);
			itm.transform.SetAsLastSibling();
			_addTableButton.transform.SetAsLastSibling();
			ResizeScrollContent();
			itm.OnStartTimer = () =>
			{
				if (!Visible)
					itm.Button.Click();
			};
			itm.Button.OnClick.RemoveAllListeners();
			itm.Button.OnClick.AddListener(() =>
			{
				OnTableClick?.Invoke(panel);
			});
			panel.OnFocus.AddListener(() =>
			{
				_tablePanels.ForEach(x => x.IsSelect = false);
				Visible = true;
				itm.IsSelect = true;
			});
			panel.OnSettings.AddListener(() =>
			{
				OnSettingsButton?.Invoke();
			});
			panel.OnTheme.AddListener(() =>
			{
				OnThemeButton?.Invoke();
			});
			panel.OnWcl.AddListener(() =>
			{
				OnWCLButton?.Invoke();
			});
			panel.OnPlayerAction.AddListener(itm.SetDateTime);
			itm.Button.Click();
		}

		public void RemoveTable(GamePanel targetGamePanel)
		{
			try
			{
				var itm = _tablePanels.Find(x => x.GamePanel == targetGamePanel);
				_tablePanels.Remove(itm);
				_addTableButton.gameObject.SetActive(_tablePanels.Count <= 3);
				itm.gameObject.SetActive(false);
				ResizeScrollContent();
			}catch{ }
		}

		private void ResizeScrollContent()
		{
			int cnt = _tablePanels.Count + 1;
			_scrollItems.content.sizeDelta = new Vector2(cnt * 145 + ((cnt - 1) * 5), _scrollItems.content.sizeDelta.y);
		}

		public void AddButtonTouch()
		{
			OnAddButton?.Invoke();
		}

		public void NextGame()
		{
			if (_tablePanels.Count <= 1) return;
			var index = _tablePanels.FindIndex(x => x.IsSelect);
			index++;
			if (index > _tablePanels.Count - 1)
				index = 0;
			_tablePanels[index].Button.Click();
		}

		public void BeforeGame()
		{
			if (_tablePanels.Count <= 1) return;
			var index = _tablePanels.FindIndex(x => x.IsSelect);
			index--;
			if (index < 0)
				index = _tablePanels.Count - 1;
			_tablePanels[index].Button.Click();
		}

	}
	public class MobileTablePictogramm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnStartTimer;

		[SerializeField] private Image _fill;
		[SerializeField] private TextMeshProUGUI _label;
		[SerializeField] private RectTransform _selectRt;

		private GamePanel _gamePanel;
		private GraphicButtonUI _button;
		private Image _selectColor;

		public GamePanel GamePanel { get => _gamePanel; set => _gamePanel = value; }
		public GraphicButtonUI Button { get => _button; set => _button = value; }
		private bool _isSelect;
		private bool _isTimer;
		private DateTime _endTime;
		private DateTime _startTime;
		private double _startDiff;

		private bool _bonderAnimateInc;
		private float _bonderAnimateAlpha;

		public bool IsSelect
		{
			get => _isSelect;
			set
			{
				if (_isSelect == value) return;

				_isSelect = value;
				_selectRt.gameObject.SetActive(_isSelect || _isTimer);
			}
		}

		private void Awake()
		{
			_fill = transform.Find("Back/Fill").GetComponent<Image>();
			_label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
			_selectRt = transform.Find("Back/Select").GetComponent<RectTransform>();
			_selectColor = _selectRt.GetComponent<Image>();
			_button = GetComponent<GraphicButtonUI>();
			_selectRt.gameObject.SetActive(false);
			_isTimer = false;
		}

		public void SetDateTime(DateTime datetime)
		{
			if (datetime == DateTime.MinValue)
			{
				StopTimer();
				return;
			}
			_startTime = GameHelper.NowTime;
			_isTimer = true;
			_endTime = datetime;
			_startDiff = (_endTime - GameHelper.NowTime).TotalSeconds;
			OnStartTimer?.Invoke();
		}

		private void StopTimer()
		{
			_isTimer = false;
			_fill.fillAmount = 0;
			_selectRt.gameObject.SetActive(_isSelect || _isTimer);
			var c = _selectColor.color;
			c.a = 1;
			_selectColor.color = c;
		}

		private void FixedUpdate()
		{
			if (!_isTimer) return;

			if (_isSelect)
			{
				_fill.fillAmount = 0;
				_selectRt.gameObject.SetActive(_isSelect || _isTimer);
				_selectColor.color = Color.white;
				return;
			}
			AnimateBorder();

			var diff = (_endTime - GameHelper.NowTime).TotalSeconds;
			if (diff <= 0)
			{
				StopTimer();
				return;
			}
			_fill.fillAmount = (float)(1 - diff / _startDiff);
		}


		private void AnimateBorder()
		{
			_selectRt.gameObject.SetActive(_isSelect || _isTimer);
			_bonderAnimateAlpha += _bonderAnimateInc ? Time.fixedDeltaTime : -Time.fixedDeltaTime;
			if (_bonderAnimateAlpha < 0 || _bonderAnimateAlpha > 1)
			{
				_bonderAnimateAlpha = Mathf.Clamp(_bonderAnimateAlpha, 0, 1);
				_bonderAnimateInc = !_bonderAnimateInc;
			}
			var c = _selectColor.color;
			c.a = _bonderAnimateAlpha;
			_selectColor.color = c;
		}

		public void SetGamePanel(GamePanel gamePanel)
		{
			_gamePanel = gamePanel;

			if (gamePanel.Table.is_vip)
			{
				_label.text = "VIP";
				return;
			}
			if (gamePanel.Table.is_dealer_choice)
			{
				_label.text = "DC";
				return;
			}
			if (gamePanel.Table.IsFaceToFace)
			{
				_label.text = "F2F";
				return;
			}

			_label.text = GameSettings.GetBlock(gamePanel.Table).ShortName;
		}

	}
}