using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
using it.Network.Rest;

public class TableListBehaviour_ : MonoBehaviour
{
	public Action<Table> SelectTableCallback;

	[SerializeField] private TableListItem tableListItemPrefab;
	[SerializeField] private Transform containerList;
	[Space][SerializeField] private TextMeshProUGUI InformationText;

	private List<Table> _tables;
	private Dictionary<ulong, TableListItem> currentTables = null;
	private Table selectTable = null;

	private bool isStartTimerRefresh = false;
	private float timeRefreshTable = 5f;
	private float timeLeftRefresh = 0f;
	public static int maxPlayersSelect = 9;
	public static bool isPrivateSelect;
	public static bool allOrNothingSelect;
	public static string nameGameType;


	void Update()
	{
		if (isStartTimerRefresh)
		{
			timeLeftRefresh -= Time.deltaTime;
			if (timeLeftRefresh < 0)
			{
				isStartTimerRefresh = false;
				RefreshFromServerCurrentTable();
			}
		}
	}

	public void Init()
	{
		GetTablesFromServer((res) =>
		{
			_tables = res;
			AddElements();
		});
	}

	private void AddElements()
	{
		if (currentTables != null)
		{
			RemoveElements();
		}

		List<GameType> selectedGames = GameHelper.GetSelectGame();
		currentTables = new Dictionary<ulong, TableListItem>();
		Table firstTable = null;
		for (int i = 0; i < _tables.Count; i++)
		{
			if (selectedGames.Contains((GameType)_tables[i].game_rule_id) && _tables[i].MaxPlayers <= maxPlayersSelect &&
					_tables[i].is_private == isPrivateSelect && _tables[i].is_all_or_nothing == allOrNothingSelect)
			{
				if (firstTable == null) firstTable = _tables[i];
				TableListItem slot = Instantiate(tableListItemPrefab);
				Transform slotTransform = slot.transform;

				slotTransform.SetParent(containerList, false);
				slot.SetTableInfo(_tables[i]);
				slot.SetCheckCallback(SelectTable);
				currentTables.Add(_tables[i].id, slot);

				if (TablesUIManager.Instance.GetSelectTable() != null && _tables[i].id == TablesUIManager.Instance.GetSelectTable().id)
				{
					SelectTableCallback?.Invoke(_tables[i]);
				}
			}
		}

		if (firstTable != null) SelectTable(firstTable);


#if !UNITY_ANDROID && !UNITY_WEBGL //there is no such thing in interface of android
		InformationText.text = nameGameType + " " + "INFORMATION".Localized();
#endif
	}

	public void RefreshFromServerCurrentTable()
	{
		if (selectTable == null || gameObject == null) return;
		GetLastUpdateTable(selectTable);
	}

	public void RefreshFromCacheTable(Table table)
	{
		if (currentTables.ContainsKey(table.id))
		{
			currentTables[table.id].SetTableInfo(table);
		}
	}

	private void SelectTable(Table table)
	{
		selectTable = table;

		foreach (var item in currentTables)
		{
			item.Value.Uncheck();
		}
		GetLastUpdateTable(table);
	}

	private void GetLastUpdateTable(Table table)
	{
		GameHelper.GetTable(table, resultTable =>
		{
			SelectTableCallback?.Invoke(resultTable);
			RefreshFromCacheTable(resultTable);
			SetTimerRefreshTable();
		}, s =>
		{

		});
	}

	private void GetTablesFromServer(Action<List<Table>> callbackSuccess, Action<string> callbackFailed = null)
	{
		TableApi.GetTables((result) =>
		{

			if (!result.IsSuccess)
			{
				callbackFailed?.Invoke(result.ErrorMessage);
				return;
			}
			callbackSuccess?.Invoke(result.Result);
		});
	}

	private void RemoveElements()
	{
		foreach (var item in currentTables) Destroy(item.Value.gameObject);
	}

	public void ChangeGameType(List<GameType> gameType)
	{
		GameHelper.SelectGame(gameType);
		AddElements();
	}


	public void ChangeTableInfo(Table table)
	{
		for (int i = 0; i < _tables.Count; i++)
		{
			if (_tables[i].id == table.id)
			{
				_tables[i] = table;
				AddElements();
				break;
			}
		}
	}


	public void SetTimerRefreshTable(bool isRun = true)
	{
		timeLeftRefresh = timeRefreshTable;
		isStartTimerRefresh = isRun;
	}
}
