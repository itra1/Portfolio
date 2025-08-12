using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;
using System.Linq;
using Sett = it.Settings;
using it.Main;
using TMPro;
using it.UI.Elements;
using System.Collections;
using System;

namespace it.Mobile.Lobby
{
	public class GameListPage : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<Table> OnInfoButton;

		[SerializeField] private it.Mobile.Lobby.LobbyFilterPanel _filter;
		[SerializeField] private GraphicButtonUI _infoButton;
		[SerializeField] private ScrollRect _scrollrect;
		[SerializeField] private TableRecordMobile _recordPrefab;
		[SerializeField] private LobbyType _lobby;
		[SerializeField] private FiltrButton[] FiltrButtons;

		public LobbyType Lobby { get => _lobby; set => _lobby = value; }

		private PoolList<TableRecordMobile> _tableRecords;
		private List<TableRecordMobile> _recordsList = new List<TableRecordMobile>();
		private List<GameType> _filtr = new List<GameType>();
		private ulong? _selectPage = null;
		private ContentSizeFitter _csf;
		private Table _selectTable;
		private string _searchString;
		private SearchLobbyType _filterType;
		private Coroutine _spawn;

		private void Awake()
		{
			_filter.OnSearch.RemoveAllListeners();
			_filter.OnSearch.AddListener(Filter);

			_infoButton.OnClick.RemoveAllListeners();
			_infoButton.OnClick.AddListener(() =>
			{
				if (_selectTable == null) return;
				OnInfoButton?.Invoke(_selectTable);
			});

			if (_scrollrect != null)
			{
				_scrollrect.onValueChanged.RemoveAllListeners();
				_scrollrect.onValueChanged.AddListener(BaseScrollchange);
			}

			for (int i = 0; i < FiltrButtons.Length; i++)
			{
				int index = i;
				FiltrButtons[index].Button.OnClick.RemoveAllListeners();
				FiltrButtons[index].Button.OnClick.AddListener(() => SetFiltr(FiltrButtons[index].Gametype));
			}
		}

		private void BaseScrollchange(Vector2 pos)
		{
			for (int i = 0; i < _recordsList.Count; i++)
			{
				var itm = _recordsList[i];
				CheckVisible(_recordsList[i]);
			}
		}

		private void CheckVisible(TableRecordMobile itm)
		{
			float offset = _scrollrect.content.anchoredPosition.y;
			itm.SetRenderer((offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height >= -_scrollrect.viewport.rect.height && offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height <= 0)
			|| (offset + itm.RectRt.anchoredPosition.y >= -_scrollrect.viewport.rect.height && offset + itm.RectRt.anchoredPosition.y <= 0)
			|| (offset + itm.RectRt.anchoredPosition.y - itm.RectRt.rect.height >= 0 && offset + itm.RectRt.anchoredPosition.y <= -_scrollrect.viewport.rect.height)
			);
		}

		[System.Serializable]
		public struct FiltrButton
		{
			//public Color StandartColor;
			//public Color SelectColor;
			public List<GameType> Gametype;
			public it.UI.Elements.FilterSwitchButtonUI Button;

			public void CheckSelect(List<GameType> game)
			{
				bool isSelect = true;

				isSelect = game.Count == Gametype.Count;

				if (isSelect)
				{
					for (int i = 0; i < game.Count; i++)
					{
						if (isSelect)
							isSelect = Gametype.Contains(game[i]);
					}
				}
				Button.IsSelect = isSelect;

				//Button.GetComponentInChildren<TextMeshProUGUI>().color = isSelect ? SelectColor : StandartColor;
				//Button.transform.Find("Select").gameObject.SetActive(isSelect);
			}
		}

		public void Filter(SearchLobbyType filter)
		{
			_filterType = filter;
			SpawnTable();
		}

		private void OnEnable()
		{
			_filterType = SearchLobbyType.None;
			_infoButton.gameObject.SetActive(false);
			_filtr.Clear();
			_recordsList.ForEach(x => x.ForceHide());
			if (FiltrButtons.Length > 0)
				SetFiltr(FiltrButtons[0].Gametype, false);

			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.TableLoad, TableLoad);
			SpawnTable();

		}
		private void SpawnTable()
		{
			if (!gameObject.activeInHierarchy) return;

			if (_spawn != null)
				StopCoroutine(_spawn);

			_spawn = StartCoroutine(SpawnTableCoroutine());
		}
		private IEnumerator SpawnTableCoroutine()
		{
			yield return new WaitForSeconds(0.05f);
			_recordPrefab.gameObject.SetActive(false);
			CompleteFiltr(_filtr);

			_csf = _scrollrect.content.GetComponent<ContentSizeFitter>();

			if (_tableRecords == null)
				_tableRecords = new PoolList<TableRecordMobile>(_recordPrefab.gameObject, _scrollrect.content);
			_tableRecords.HideAll();

			_recordsList.ForEach(x => x.gameObject.SetActive(false));

			//_csf.enabled = false;
			//yield return null;
			//_csf.enabled = true;

			RectTransform recordRect = _recordPrefab.GetComponent<RectTransform>();

			List<Table> useTables = TableManager.Instance.TableList.OrderBy(x => x.id).ToList().OrderByDescending(x=>x.table_player_sessions.Length).ToList();

			if (useTables.Count == 0) yield break;


			bool existsSelect = false;
			_selectPage = null;
			var itm = Sett.GameSettings.Blocks.Find(x => x.Lobby == Lobby);
			int cnt = 0;
			for (int i = 0; i < useTables.Count; i++)
			{
				if (itm.IsVip && useTables[i].is_vip != itm.IsVip)
					continue;
				if (!itm.IsVip)
				{
					if (useTables[i].is_dealer_choice != itm.IsDealerChoice) continue;

					if (!itm.IsVip && !itm.IsDealerChoice)
					{
						if (useTables[i].is_all_or_nothing != itm.AllOrNothing) continue;
						if (useTables[i].is_private != itm.IsPrivate) continue;
						if (useTables[i].MaxPlayers > itm.MaxPlayers || useTables[i].MaxPlayers < itm.MinPlayers) continue;
					}
				}

				if (!string.IsNullOrEmpty(_searchString) && !itm.Name.Contains(_searchString)) continue;

				if (!itm.TypeGame.Contains((GameType)useTables[i].game_rule_id)) continue;

				if (_filtr.Count != 0 && !_filtr.Contains((GameType)useTables[i].game_rule_id)) continue;

				if ((_filterType & SearchLobbyType.FreeTables) != 0)
					if (useTables[i].table_player_sessions.Length >= useTables[i].MaxPlayers) continue;
				bool isNameInFilter = false;
				bool existsNameFilter = false;

				if ((_filterType & SearchLobbyType.Micro) != 0)
				{
					existsNameFilter = true;
					if (!isNameInFilter)
						isNameInFilter = useTables[i].name.ToLower().Contains("micro");
				}
				if ((_filterType & SearchLobbyType.Average) != 0)
				{
					existsNameFilter = true;
					if (!isNameInFilter)
						isNameInFilter = useTables[i].name.ToLower().Contains("average");
				}
				if ((_filterType & SearchLobbyType.High) != 0)
				{
					existsNameFilter = true;
					if (!isNameInFilter)
						isNameInFilter = useTables[i].name.ToLower().Contains("high");
				}

				if (existsNameFilter && !isNameInFilter) continue;

				cnt++;

				GetType(useTables[i]);

				var rec = _recordsList.Find(x => !x.gameObject.activeSelf && x.Table != null && x.Table.id == useTables[i].id);
				if (rec == null)
				{
					rec = _tableRecords.GetItem();
					_recordsList.Add(rec);

					rec.OnClick = (r) =>
					{
						//InfoButtonTouch(r);
						_selectTable = r.Table;
						_infoButton.gameObject.SetActive(true);
						if (_selectPage == r.Table.id) return;
						_selectPage = r.Table.id;
						ConfirmSelect();
					};

					rec.OnSizeChange = () =>
					{
						_csf.enabled = false;
						_csf.enabled = true;
					};

					rec.OnOpen = (table) =>
					{
						TableManager.Instance.OpenTable(table.id);
						//OpenTableManager.Instance.OpenGame(table);
					};

					yield return null;
					_scrollrect.content.sizeDelta = new Vector2(_scrollrect.content.sizeDelta.x, (_recordPrefab.HeaderRect.rect.height-10) * cnt);
				}
				rec.transform.SetSiblingIndex(cnt);
				rec.SetRenderer(true);
				rec.SetData(useTables[i]);
				rec.gameObject.SetActive(true);

				//rec.RectRt.anchoredPosition = new Vector2(rec.RectRt.anchoredPosition.x, cnt * rec.HeaderRect.rect.height);

				if (useTables[i].id == _selectPage)
					existsSelect = true;
			}
			_csf.enabled = false;
			yield return null;
			_csf.enabled = true;

			_scrollrect.content.sizeDelta = new Vector2(_scrollrect.content.sizeDelta.x, (_recordPrefab.HeaderRect.rect.height - 10) * cnt);
#if UNITY_STANDALONE
			//if (!existsSelect && records.Count > 0)
			//{
			//	_selectPage = records[0].Id;
			//	ConfirmSelect();
			//	//TablePanel.SetTableRecord(Lobby, records[0]);
			//}
#endif
			yield return null;
			BaseScrollchange(Vector2.zero);
		}

		private void ConfirmSelect()
		{

			for (int i = 0; i < _recordsList.Count; i++)
			{
				//_recordsList[i].Focus(_recordsList[i].Table.Id == _selectPage);
			}
			SocketClient.Instance.EnterTableChanel((ulong)_selectPage);
		}


		private void TableLoad(com.ootii.Messages.IMessage handle)
		{
			SpawnTable();
		}

		protected virtual void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.Holdem;
		}

		public void SetFiltr(List<GameType> filtr, bool respawn = true)
		{
			this._filtr = new List<GameType>(filtr);
			if (respawn)
				SpawnTable();
		}

		public virtual void CompleteFiltr(List<GameType> filtr)
		{
			for (int i = 0; i < FiltrButtons.Length; i++)
			{
				FiltrButtons[i].CheckSelect(filtr);
			}
		}

		// Вызов информационного окна
		//public void InfoButtonTouch(TableRecordMobile tableRecordMobile)
		//{
		//	it.Logger.Log(tableRecordMobile);

		//	tableRecordMobile.ToggleVisible();
		//}

	}
}
