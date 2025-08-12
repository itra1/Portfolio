using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Network.Rest;
using Leguar.TotalJSON;
using it.Popups;
using it.Main;
using it.Mobile;
using System.Threading;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;

public class TableManager : Singleton<TableManager>
{
	public static UnityEngine.Events.UnityEvent<Table> OnUpdateRecord = new UnityEngine.Events.UnityEvent<Table>();

	public List<Table> TableList { get => _tableList; set => _tableList = value; }

	private List<Table> _tableList = new List<Table>();
	private ulong? _selectTable;
	private Coroutine _updateRecord;
	[ColorUsage(true, true)]
	private Color _c;
	private CancellationTokenSource _listUpdateCancellationTokenSource;
	private CancellationToken _listUpdateCancellationToken;

	private void OnEnable()
	{

		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);
		if (ServerManager.ExistsServers)
			LoadsTables();
		else
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ServersLoaded, LoadServersComplete);
		}
	}

	public bool CheckActiveTableId(ulong tableId)
	{

#if UNITY_STANDALONE

		return StandaloneController.Instance.TableWindows.Find(x => x.Id == tableId) != null;

#else
		return OpenTableManager.Instance.OpenPanels.ContainsKey(tableId);
#endif

	}

	private void LoadServersComplete(com.ootii.Messages.IMessage handler)
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ServersLoaded, LoadServersComplete);
		LoadsTables();
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
	}

	public void SetSelectTable(ulong selectTable)
	{
		if (_selectTable != null && _selectTable == selectTable) return;

		_selectTable = selectTable;

		  UpdateRecordAsync();

		//if (_updateRecord != null)
		//	StopCoroutine(_updateRecord);
		//_updateRecord = StartCoroutine(UpdateRecordCoroutine());
	}

	private void UserLogin(com.ootii.Messages.IMessage handle)
	{
		//if (_selectTable != null)
		//	LoadTableRecord();
	}

	private void LoadsTables()
	{
		_selectTable = AppConfig.TableExe;
		if (_selectTable == null)
			AllUpdateAsync();
		//StartCoroutine(AllUpdateCoroutine());
		//else
		//	LoadTableRecord();
		UpdateRecordAsync();
		//_updateRecord = StartCoroutine(UpdateRecordCoroutine());
	}

	public void LoadTables()
	{
		TableApi.GetTables((result) =>
		{

			if (!result.IsSuccess)
			{
				it.Logger.Log("Error request complete " + result.ErrorMessage);
				return;
			}
			_tableList = result.Result;
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.TableLoad, 0.001f);

		});

	}

	public async void AllUpdateAsync()
	{
		while (true)
		{
			LoadTables();
			await UniTask.Delay(30000);
		}
	}
	public async void UpdateRecordAsync()
	{
		while (true)
		{
			if (_selectTable != null)
			{
				LoadTableRecord();
				await UniTask.Delay(20000);
			}
			await UniTask.Delay(1000);
		}
	}

	public void LoadTableRecord()
	{
		LoadTableRecord((ulong)_selectTable);
	}

	public void LoadTableRecord(ulong tableId, UnityEngine.Events.UnityAction<Table> OnComplete = null)
	{
		TableApi.GetTable(tableId, (result) =>
		{
			if (result.IsSuccess)
			{
				UpdateOrCreateTable(result.Result);
				OnComplete?.Invoke(result.Result);
			}
			else
			{

			}
		});
	}

	private void UpdateOrCreateTable(Table table)
	{

		Table t = _tableList.Find(x => x.id == table.id);

		if (t != null)
			t.Update(table);
		else
			_tableList.Add(table);
		OnUpdateRecord?.Invoke(table);
	}

	public void AddTable(Table tb)
	{
		_tableList.Add(tb);
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.TableLoad, 0.001f);
	}

	public Table GetTableById(ulong id)
	{
		if (_tableList.Count == 0)
			return null;
		return _tableList.Find(x => x.id == id);
	}

	public void OpenTable(ulong tableId, bool join = false)
	{
		if (!CheckAlowedOpenTable())
			return;

		var tb = GetTableById(tableId);

		if (tb == null) return;

		if (tb.is_vip)
		{
			if (!tb.isAtTheTable)
			{
				TablePinPopup panel = it.Main.PopupController.Instance.ShowPopup<TablePinPopup>(PopupType.TablePin);
				panel.Set(tb);
				panel.OnConfirm = (pass) =>
				{
					TableManager.Instance.OpenTable(tb, pass, join);
				};
			}
			else
				TableManager.Instance.OpenTable(tb, "", join);
		}
		else
		{
			TableApi.ObserveTable(tb.id, "", (result) =>
			{
				TableManager.Instance.OpenTable(tb, "", join);
			});
		}
	}

	public static bool CheckAlowedOpenTable(){

		if (AppConfig.IsLockedOpenTable)
		{
			it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("popup.update.tableLock".Localized());
			return false;
		}
		if (!UserController.IsLogin)
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
			return false;
		}
		return true;
	}

	public void CreateTable()
	{

		if (!UserController.IsLogin)
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
			return;
		}

		PopupController.Instance.ShowPopup(PopupType.CreateTable);
	}

	public void CreateTableReauest(it.Network.Rest.CreateTableInfo info, UnityEngine.Events.UnityAction<bool> onComplete)
	{
		TableApi.CreateTable(info, (result) =>
		{
			if (result.IsSuccess)
			{
				onComplete?.Invoke(true);
				AddTable(result.Result);
				//_title.text = $"{"popup.createTable.tableName".Localized()}: {result.Result.Name}";
				TableManager.Instance.OpenTable(result.Result, info.password);
			}
			else
			{
				onComplete?.Invoke(false);
				//_title.text = "popup.createTable.tableNameError".Localized();
			}

		});
	}

	public void OpenTable(Table targetTable, string password = "", bool join = false)
	{
#if UNITY_STANDALONE && !UNITY_EDITOR

		var windowData = StandaloneController.Instance.TableWindows.Find(x => x.Id == targetTable.id);
		if (windowData != null)
		{
			StandaloneController.Instance.FocusWindow(targetTable);
		}
		else
		{
			StandaloneController.OpenNewTableWindow(targetTable, password, join);
		}
		return;

#else
		PlayerPrefs.SetString(StringConstants.TablePassword(targetTable.id), password);
		UserController.AutoJoin = join;

#if UNITY_STANDALONE
		AppManager.Instance.OpenGame(targetTable.id);
#else
		LoadTableRecord(targetTable.id);
		OpenTableManager.Instance.OpenGame(targetTable);

#endif

#endif

	}

}