using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using it.Inputs;
using Sett = it.Settings;
using DG.Tweening;
using it.UI;
using Garilla.Games;
using it.Animations;
using it.Main.SinglePages;
using UnityEngine.Events;

namespace it.Mobile.Lobby
{
	public class TableRecordMobile : MonoBehaviour, MyTableRecord
	{
		public UnityEngine.Events.UnityAction OnSizeChange;
		public UnityEngine.Events.UnityAction<TableRecordMobile> OnClick;
		public UnityEngine.Events.UnityAction<Table> OnOpen;
		UnityAction<Table> MyTableRecord.OnOpenEvent { get => OnOpen; set => OnOpen = value; }
		UnityAction MyTableRecord.OnSizeChangeEvent { get => OnSizeChange; set => OnSizeChange = value; }
		public static TableRecordMobile _openRecord;

		[SerializeField] private CanvasGroup _body;
		[SerializeField] private RectTransform _tableParent;
		[SerializeField] private PlayerGameIcone _gamePlayerUIPrefabs;
		[SerializeField] private GamePlaceUI _gamePlayerPlace;
		[SerializeField] private RectTransform _headerRect;
		[SerializeField] private RectTransform _bodyRect;
		[SerializeField] private Image _lightImage;
		//  [SerializeField] private RectTransform _tabletParentRect;
		[SerializeField] private TextMeshProUGUI _nameLabel;
		[SerializeField] private TMValueIntAnimate _playersCurrentLabel;
		[SerializeField] private TextMeshProUGUI _playersMaxLabel;
		[SerializeField] private ImageCircleFillAmount _playersDiagramm;
		[SerializeField] private CurrencyLabel _stakesLabel;
		[SerializeField] private CurrencyLabel _buyInLabel;
		public Table Table { get => _table; set => _table = value; }

		//private List<PlayerGameIcone> _players = new List<PlayerGameIcone>();
		private PoolList<PlayerGameIcone> _playerPooler;
		private PoolList<GamePlaceUI> _playerPlacePooler;
		private it.UI.TableInfo _tablet;
		private Table _table;
		//private LobbyType _lobbyType;
		private string _chanel;
		private RectTransform _rt;
		private bool _isExtend;
		private TablePlaceManager _placeManager;
		public RectTransform RectRt
		{
			get
			{
				if (_rt == null)
					_rt = GetComponent<RectTransform>();
				return _rt;
			}
		}

		public RectTransform HeaderRect { get => _headerRect; set => _headerRect = value; }

		public void OnClickTable()
		{
			OnClick?.Invoke(this);
		}

		public void SetRenderer(bool isVisible)
		{
			if (_body == null) return;
			_body.alpha = isVisible ? 1 : 0;
		}

		private void Awake()
		{
			if (_rt == null)
				_rt = GetComponent<RectTransform>();
			_rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _headerRect.rect.height);
			_tableParent.gameObject.SetActive(false);
			ForceHide();
		}

		public void SetData(Table record)
		{
			bool changeTable = _table == null || _table.id != record.id;

			if (_table != null)
			{
				_table.OnUpdateEvent.RemoveListener(UpdateRecord);
				RemoveListeners();
			}

			if (changeTable)
				ForceHide();

			_table = record;
			//_lobbyType = lobbyType;
			_chanel = SocketClient.GetChanelTable(_table.id);

			_table.OnUpdateEvent.AddListener(UpdateRecord);
			AddListeners();

			//com.ootii.Messages.MessageDispatcher.SendMessage(this,
			//EventsConstants.GameTableUpdate(_chanel), this, 0.05f);
			//HideExtend();
			//if (changeTable)
				ConfirmData();
		}

		private void OnDestroy()
		{
			if (!string.IsNullOrEmpty(_chanel))
				RemoveListeners();
		}

		private void RemoveListeners()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableUpdate(_chanel), OnTableReserve);
		}

		private void AddListeners()
		{
			RemoveListeners();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableUpdate(_chanel), OnTableReserve);
		}

		private void ConfirmData()
		{

			var cColor = _table.is_dealer_choice
			? Sett.GameSettings.Colors.Find(x => x.TypeGame.Contains(GameType.DealerChoice))
			: Sett.GameSettings.Colors.Find(x => x.TypeGame.Contains((GameType)_table.game_rule_id));

			string tableNameType = cColor.Name;
			if (_table.is_all_or_nothing && tableNameType != "Holdem")
				tableNameType = "Omaha";

			Color recColor = cColor.Color;
			if (_nameLabel != null)
			{
				_nameLabel.GetComponent<TextMeshProUGUI>().color = recColor;
				_nameLabel.text = $"{tableNameType} <color=#ffffff><size=62%> {_table.name}";
#if UNITY_EDITOR
				_nameLabel.text = _table.id + " : " + _nameLabel.text;
#endif
			}
			if (_stakesLabel != null)
			{
				if (_table.ante != null && _table.ante > 0)
					_stakesLabel.SetValue("{0} (A{1})", (float)_table.ante, (float)_table.ante);
				else
					_stakesLabel.SetValue("{0} / {1}", _table.SmallBlindSize, _table.big_blind_size);
				_stakesLabel.GetComponent<TextMeshProUGUI>().color = recColor;
			}

			if (_playersCurrentLabel != null)
			{
				_playersCurrentLabel.StartValue = _playersCurrentLabel.EndValue;
				_playersCurrentLabel.EndValue = _table.table_player_sessions.Length;
			}
			//_playersCurrentLabel.text = _table.TablePlayerSessions.Length.ToString();

			if (_playersMaxLabel != null)
				_playersMaxLabel.text = _table.MaxPlayers.ToString();

			if (_playersDiagramm != null)
			{
				_playersDiagramm.StartValue = _playersDiagramm.CurrentValue;
				_playersDiagramm.EndValue = (float)_table.table_player_sessions.Length == 0 ? 0 : (float)_table.table_player_sessions.Length / (float)_table.MaxPlayers;
			}
			if (_buyInLabel != null)
				_buyInLabel.SetValue(_table.BuyInMinEURO);

		}

		private void HightItemInContent(float hight)
		{
			/*_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x,
				 (hight * rects.Length));*/

		}

		public void ForceHide()
		{

			if (_rt == null)
				_rt = GetComponent<RectTransform>();
			_rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _headerRect.rect.height);
			_isExtend = false;
			_lightImage.gameObject.SetActive(false);
		}

		public void HideExtend()
		{
			// rects = _tabletParentRect.GetComponentsInChildren<RectTransform>();

			if (_rt == null)
				_rt = GetComponent<RectTransform>();

			if (_rt.rect.height == _headerRect.rect.height) return;

			_rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, _headerRect.rect.height), 0.3f).OnUpdate(() =>
			{
				OnSizeChange?.Invoke();
			}).OnComplete(() =>
			{
				OnSizeChange?.Invoke();
				_tableParent.gameObject.SetActive(false);
			});

			HightItemInContent(_bodyRect.rect.height);

			_isExtend = false;
			_lightImage.gameObject.SetActive(false);
		}

		public void VisibleExtend()
		{

			if (_isExtend) return;
			_tableParent.gameObject.SetActive(true);
			OnClick?.Invoke(this);

			if (_rt == null)
				_rt = GetComponent<RectTransform>();

			if (_placeManager == null)
			{
//#if UNITY_WEBGL
//				var placePref = Garilla.WebGL.WebGLResources.AndroidSettings.GetLobbyTablePlaceManager(_table);
//#else
				var placePref = Sett.AndroidSettings.GetLobbyTablePlaceManagerStatic(_table, false);
				//#endif

				var ppObject = Instantiate(placePref.gameObject, _bodyRect);
				_placeManager = ppObject.GetComponent<TablePlaceManager>();
				RectTransform pmRect = _placeManager.GetComponent<RectTransform>();
				pmRect.anchorMin = Vector2.zero;
				pmRect.anchorMax = Vector2.one;
				pmRect.localPosition = Vector3.zero;
				pmRect.anchoredPosition = Vector3.zero;
				pmRect.localScale = Vector3.one;
				pmRect.sizeDelta = Vector2.zero;
				_placeManager.SetTablePositions(_table);
			}

			//TableManager.OnUpdateRecord = (t) =>
			//{
			//	if (_table.Id != t.Id) return;

			//	_table = t;
			//	ConfirmData();
			//};
			TableManager.Instance.LoadTableRecord(_table.id);

			if (_openRecord != null && _openRecord != this)
				_openRecord.HideExtend();
			_openRecord = this;

			_lightImage.gameObject.SetActive(true);

			_rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, _bodyRect.rect.height + _headerRect.rect.height), 0.3f).OnUpdate(() =>
			{
				OnSizeChange?.Invoke();
			}).OnComplete(() =>
			{
				OnSizeChange?.Invoke();
			});
			_isExtend = true;
			ShowExtendData();
			SpawnItems();
			// HightItemInContent(_infoBlock.rect.height + _tabletParentRect.rect.height);
			SocketClient.Instance.EnterTableChanel((ulong)_table.id);
		}

		private void UpdateRecord(UpdatedMaterial updateTable)
		{

			Table t = (Table)updateTable;

			if (_table.id != t.id) return;

			_table = t;
			ConfirmData();
			if (_isExtend)
				SpawnItems();
		}

		private void OnTableReserve(com.ootii.Messages.IMessage handle)
		{
			//var tableEvent = ((it.Network.Socket.GameUpdate)handle.Data).TableEvent;
			//ReservedPlaces(tableEvent.table.TablePlayerReservations);
			//ConfirmData();
			//SpawnItems();

			TableManager.Instance.LoadTableRecord(_table.id);

		}

		private void SpawnItems()
		{
			if (_playerPooler == null)
				_playerPooler = new PoolList<PlayerGameIcone>(_gamePlayerUIPrefabs, _bodyRect);
			_playerPooler.HideAll();
			if (_playerPlacePooler == null)
				_playerPlacePooler = new PoolList<GamePlaceUI>(_gamePlayerPlace, _bodyRect);
			_playerPlacePooler.HideAll();

			for (int i = 0; i < _placeManager.Places.PlayerPlaces.Count; i++)
			{
				int place = i;
				if (!Table.IsFreePlace(place))
				{
					var player = Table.GetPlayerByPlace(place);
					var item = _playerPooler.GetItem();
					var rect = item.GetComponent<RectTransform>();
					rect.SetParent(_placeManager.Places.PlayerPlaces[i]);
					rect.transform.localPosition = Vector3.zero;
					rect.transform.localScale = Vector3.one;
					item.Set(player);
				}
				else
				{
					var placePrefab = _playerPlacePooler.GetItem();
					placePrefab.IsInverce = true;
					var placeRect = placePrefab.GetComponent<RectTransform>();
					placePrefab.SetReserved(Table.IsReservePlace(place));
					placeRect.SetParent(_placeManager.Places.PlayerPlacesTakeSit[i]);
					placeRect.transform.localPosition = Vector3.zero;
					placeRect.transform.localScale = Vector3.one;
				}
			}

		}
		public void ToggleVisible()
		{
			if (_isExtend)
				HideExtend();
			else
				VisibleExtend();
		}

		private void ShowExtendData()
		{

			if (_tablet != null) return;

			//TableInfo ti = it.Settings.GameSettings.MobileTablesLobbyPrefab.Find(x => x.LobbyType == _lobbyType && (x.LobbyType != LobbyType.Plo || x.GameType.Contains((GameType)_table.GameRuleId)));
			TableInfo ti = null;

			if (ti == null) return;

			var inst = Instantiate(ti.gameObject, _bodyRect);
			inst.transform.localPosition = Vector3.zero;
			inst.transform.localScale = Vector3.one;
			inst.transform.localRotation = Quaternion.identity;

			RectTransform instaRt = inst.GetComponent<RectTransform>();
			instaRt.anchoredPosition = Vector3.zero;
			instaRt.anchorMin = Vector3.zero;
			instaRt.anchorMax = Vector3.one;
			instaRt.sizeDelta = Vector3.zero;
			instaRt.SetAsFirstSibling();

			_tablet = inst.GetComponent<it.UI.TableInfo>();

			ti.gameObject.SetActive(true);
			ti.SetData(_table);
		}

		public void OpenButtonTouch()
		{
			OnOpen?.Invoke(_table);
		}

	}
}
