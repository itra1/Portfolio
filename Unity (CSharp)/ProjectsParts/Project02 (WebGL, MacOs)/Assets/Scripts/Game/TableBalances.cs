using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using it.Network.Rest;
using com.ootii.Geometry;
using Garilla.Games;

namespace it.UI
{
	public class TableBalances : MonoBehaviour
	{
		[SerializeField] private RectTransform _content;
		[SerializeField] private ScrollRect _scroll;
		[SerializeField] private RectTransform _myItemRect;
		[SerializeField] private RectTransform _playerItemRect;
		[SerializeField] private Color _incWinColor;
		[SerializeField] private Color _decWinColor;
		[SerializeField] private RectTransform _badBeatPanel;
		[SerializeField] private RectTransform _promotionsIcons;

		private GamePanel _gamePanel;
		private TableBalancesItem _myItem;
		private TableBalancesItem _playerItem;
		private PoolList<TableBalancesItem> _itemsPooler;
		private Vector3 _startAncor;
		private RectTransform _rt;
		private bool _isVisible;
		private CanvasGroup _canv;
		private TopMobilePanel _topMobilePanel;
		private int _countVisible;

		public GamePanel GamePanel { get => _gamePanel; set => _gamePanel = value; }

		private void Awake()
		{
			_canv = gameObject.GetOrAddComponent<CanvasGroup>();
			gameObject.GetOrAddComponent<GraphicRaycaster>();
			_canv.alpha = 0;

			_rt = GetComponent<RectTransform>();
			_startAncor = _rt.anchoredPosition;
			_myItem = _myItemRect.gameObject.AddComponent<TableBalancesItem>();
			_playerItem = _playerItemRect.gameObject.AddComponent<TableBalancesItem>();

			if (_itemsPooler == null)
				_itemsPooler = new PoolList<TableBalancesItem>(_playerItem.gameObject, _scroll.content);
			_playerItem.gameObject.SetActive(false);
			_myItem.gameObject.SetActive(false);
			_isVisible = false;

#if !UNITY_STANDALONE
			if (_topMobilePanel == null)
				_topMobilePanel = GetComponentInChildren<TopMobilePanel>(true);
#endif

		}

		private void Start()
		{
			if (_promotionsIcons != null)
			{
				var pip = _promotionsIcons.GetComponent<Garilla.Promotions.PromotionsIconePanel>();
				pip.GameManager = GamePanel.CurrentGameUIManager;
				pip.SelectTable(GamePanel.Table);
			}
		}

		public void SetVisible()
		{
			_countVisible++;
			if (_isVisible) return;
			_isVisible = true;

			if (_badBeatPanel != null)
			{
				var bbWindet= _badBeatPanel.GetComponent<it.UI.Elements.BadBeatWidget>();
				if(_countVisible == 1)
					bbWindet.Clear();
				bbWindet.GetData();

				if (it.Settings.GameSettings.BadBeat.GameTypes.Contains((GameType)GamePanel.Table.game_rule_id))
				{
					_badBeatPanel.gameObject.SetActive(true);
					_content.anchoredPosition = new Vector2(0, -322);
					_content.sizeDelta = new Vector2(0, -322 - (_promotionsIcons != null ? _promotionsIcons.rect.height : 0));
				}
				else
				{
					_badBeatPanel.gameObject.SetActive(false);
					_content.anchoredPosition = new Vector2(0, -116);
					_content.sizeDelta = new Vector2(0, -116- (_promotionsIcons != null ? _promotionsIcons.rect.height : 0));
				}
			}

#if !UNITY_STANDALONE
			var tmp = transform.GetComponentInChildren<TopMobilePanel>();
			if (tmp != null)
				tmp.OnBackAction.AddListener(SetHide);
			_topMobilePanel.SwipeListenerAdd();
#endif
			_canv.alpha = 1;
			Spawn();
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

		private void Spawn()
		{
			_itemsPooler.HideAll();
			var shareData = GamePanel.CurrentGameUIManager._currentSharedData;

			if (shareData == null)
			{
				_myItem.gameObject.SetActive(false);

				foreach (var p in _gamePanel.CurrentGameUIManager.Players)
				{
					if (p.Value.UserId == UserController.User.id)
					{
						_myItem.gameObject.SetActive(value: true);
						_myItem.SetData(p.Value, _incWinColor);
					}
				}

				return;
			}
			var userList = shareData.players.OrderByDescending(x => x.session_stat.balance).ToList();

			for (int i = 0; i < userList.Count; i++)
			{
				TableBalancesItem tbi = null;

				if (userList[i].user.id == UserController.User.id)
				{
					tbi = _myItem;
					tbi.transform.SetAsFirstSibling();
					tbi.gameObject.SetActive(true);
				}
				else
				{
					tbi = _itemsPooler.GetItem();
				}
				tbi.SetData(userList[i].user.nickname, userList[i].session_stat, userList[i].session_stat.balance > 0 ? _incWinColor : _decWinColor);
			}
			_scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, userList.Count * (_myItem.GetComponent<RectTransform>().sizeDelta.y + 5));
		}

	}
	public class TableBalancesItem : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _nameLabel;
		[SerializeField] private TextMeshProUGUI _buyInLabel;
		[SerializeField] private TextMeshProUGUI _winLabel;

		private void Awake()
		{
			_nameLabel = transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
			_buyInLabel = transform.Find("BuyIn").gameObject.GetComponent<TextMeshProUGUI>();
			_winLabel = transform.Find("Win").gameObject.GetComponent<TextMeshProUGUI>();
		}

		public void SetData(string nickname, UserSessionStat stat, Color winColor)
		{
			_nameLabel.text = nickname;
			_buyInLabel.text = it.Helpers.Currency.String(stat.total_buy_in, false);
			_winLabel.text = it.Helpers.Currency.String(stat.balance, false);
			_winLabel.color = winColor;
		}
		public void SetData(PlayerGameIcone player, Color winColor)
		{
			_nameLabel.text = player.TablePlayerSession.user.nickname;
			_buyInLabel.text = it.Helpers.Currency.String(player.TablePlayerSession.amount, false);
			_winLabel.text = it.Helpers.Currency.String(0, false);
			_winLabel.color = winColor;
		}
	}
}