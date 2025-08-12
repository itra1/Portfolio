using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;
using System.Linq;
using it.UI;
using Sett = it.Settings;
using System;
using it.Popups;
//using System.Threading.Tasks;

namespace it.Main
{
	public abstract class SheetBase : UIPanel
	{
		[SerializeField] private ScrollRect _scrollrect;
		[SerializeField] private TableRecord _recordPrefab;
		[SerializeField] private LobbyType _lobby;
		[SerializeField] private FiltrButton[] FiltrButtons;

		private PoolList<TableRecord> _recordPooler;
		private List<TableRecord> _recordsList = new List<TableRecord>();
		private ulong? _selectPage = null;
		private List<GameType> _filtr = new List<GameType>();
		private TablePanel _tablePanel;
		private string _searchString;
		private Table _selectRecord;

		public TablePanel TablePanel
		{
			get
			{
				if (_tablePanel == null)
					_tablePanel = GetComponentInChildren<TablePanel>();
				return _tablePanel;
			}
			set
			{
				_tablePanel = value;
			}
		}
		[System.Serializable]
		public struct FiltrButton
		{
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
			}
		}


		public void Search(string newSearch)
		{
			_searchString = newSearch;
			SpawnTable();
		}

		public LobbyType Lobby { get => _lobby; set => _lobby = value; }

		protected virtual void OnEnable()
		{
			_searchString = "";

			_filtr.Clear();

			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.TableLoad, TableLoad);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.TableLoad, TableLoad);
			
			for (int i = 0; i < FiltrButtons.Length; i++)
			{
				FiltrButtons[i].Button.OnClick.RemoveAllListeners();
				List<GameType> l = new List<GameType>(FiltrButtons[i].Gametype);
				FiltrButtons[i].Button.OnClick.AddListener(() => SetFiltr(l));
			}
			
			if (FiltrButtons.Length > 0)
			{
				SetFiltr(new List<GameType>(FiltrButtons[0].Gametype));
			}
			else
			{
				SpawnTable();
			}
		}

		protected void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.TableLoad, TableLoad);
		}

		private void TableLoad(com.ootii.Messages.IMessage handle)
		{
			SpawnTable();
		}

		protected virtual void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.Holdem;
		}

		public void SetFiltr(List<GameType> filtr)
		{
			this._filtr = filtr;
			_selectPage = null;
			CompleteFiltr(_filtr);
			SpawnTable();
		}

		public virtual void CompleteFiltr(List<GameType> filtr)
		{
			for (int i = 0; i < FiltrButtons.Length; i++)
			{
				FiltrButtons[i].CheckSelect(filtr);
			}
		}
		private void SpawnTable()
		{
			if (!gameObject.activeInHierarchy) return;
			StartCoroutine(SpawnTableCoroutine());
		}
		private IEnumerator SpawnTableCoroutine()
		{
			_recordPrefab.gameObject.SetActive(false);

			yield return new WaitForSeconds(0.05f);
			var vlg = _scrollrect.content.GetComponent<VerticalLayoutGroup>();
			vlg.padding.top = 6;
			vlg.padding.bottom = 6;

			if (_recordPooler == null)
				_recordPooler = new PoolList<TableRecord>(_recordPrefab, _recordPrefab.transform.parent);

			_recordPooler.HideAll();

			_recordsList.ForEach(x => x.gameObject.SetActive(false));

			TablePanel.SetTableRecord(Lobby, null);

			RectTransform recordRect = _recordPrefab.GetComponent<RectTransform>();

			List<Table> useTables = TableManager.Instance.TableList.OrderBy(x => x.id).ToList().OrderByDescending(x => x.table_player_sessions.Length).ToList();

			if (useTables.Count == 0) yield break;

			bool existsSelect = false;
			//_selectPage = null;
			var itm = Sett.GameSettings.Blocks.Find(x => x.Lobby == Lobby);
			int cnt = 0;
			TableRecord firstActiveTable = null;
			for (int i = 0; i < useTables.Count; i++)
			{
				var useTable = useTables[i];
				if (/*itm.IsVip &&*/ useTables[i].is_vip != itm.IsVip)
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

				cnt++;

				GetType(useTables[i]);
				TableRecord rec = null;
				if (_recordsList != null && _recordsList.Count > 0)
				{
					rec = _recordsList.Find(x =>
						x != null && 
						x.gameObject.activeSelf && 
						x.Table != null && 
						x.Table.id == useTables[i].id);
				}

				if (rec == null)
				{
					rec = _recordPooler.GetItem();
					_recordsList.Add(rec);
					rec.OnClick = (r) =>
					{
						if (_selectPage != null && _selectPage == r.Table.id) return;
						_selectPage = r.Table.id;
						ConfirmSelect();
						//TablePanel.SetTableRecord(Lobby, r.Table);
						SelectRecord(Lobby, r);
					};
					rec.OnDoubleClick = (r) =>
					{
						TableManager.Instance.OpenTable(r.Table.id);
					};
					
					_scrollrect.content.sizeDelta = new Vector2(_scrollrect.content.sizeDelta.x, (recordRect.rect.height + 6) * cnt + 6);
				}
				rec.transform.SetSiblingIndex(cnt);
				rec.SetData(useTables[i]);
				rec.gameObject.SetActive(true);
				if (firstActiveTable == null) firstActiveTable = rec;
				if (useTables[i].id == _selectPage)
				{
					existsSelect = true;
					ConfirmSelect();
					SelectRecord(Lobby, rec);
				}
			}
			_scrollrect.content.sizeDelta = new Vector2(_scrollrect.content.sizeDelta.x, (recordRect.rect.height + 6) * cnt + 6);

			if (cnt == 0)
			{
				TablePanel.SetTableRecord(Lobby, null);
			}

			if (!existsSelect && firstActiveTable != null && firstActiveTable.Table.id != _selectPage)
			{
				_selectPage = firstActiveTable.Table.id;
				ConfirmSelect();
				SelectRecord(Lobby, firstActiveTable);
			}
			
			CompleteFiltr(_filtr);
		}

		private void SelectRecord(LobbyType lobby, TableRecord record)
		{
			TablePanel.SetTableRecord(Lobby, record.Table);
			_selectRecord = record.Table;

			_selectRecord.OnUpdateEvent.RemoveListener(UpdateRecord);
			_selectRecord.OnUpdateEvent.AddListener(UpdateRecord);

			//TableManager.OnUpdateRecord = (t) =>
			//{
			//	if (_selectRecord.Id != t.Id) return;
			//	TablePanel.SetTableRecord(Lobby, t);
			//	record.SetData(t);

			//	//if(!t.isAtTheTable){
			//	//	ObserveAndOpenTable(t, null);
			//	//}

			//};
			TableManager.Instance.SetSelectTable(_selectRecord.id);

		}

		private void UpdateRecord(UpdatedMaterial updateTable)
		{
			Table t = (Table)updateTable;

			if (_selectRecord.id != t.id) return;
			TablePanel.SetTableRecord(Lobby, t);
		}

		private void ConfirmSelect()
		{
			if (_selectPage == null) return;

			for (int i = 0; i < _recordsList.Count; i++)
			{
				if (_recordsList[i].Table != null)
					_recordsList[i].Focus(_recordsList[i].Table.id == _selectPage);
			}
			SocketClient.Instance.EnterTableChanel((ulong)_selectPage);
		}

		private void ObserveAndOpenTable(Table table, Action<Table> callback)
		{
			GameHelper.ObserveTable(table, "", (tableResponse) =>
			{
				callback?.Invoke(table);
			},
			(error) =>
			{
				//SSTools.ShowMessage("Failed loading table", SSTools.Position.bottom, SSTools.Time.twoSecond);
			});
		}
	}
}