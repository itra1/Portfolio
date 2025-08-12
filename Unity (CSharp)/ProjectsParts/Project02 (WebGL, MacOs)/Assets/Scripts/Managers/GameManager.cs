using DG.Tweening;

using it.Network.Rest;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using static it.Settings.AppSettings;

public class GameManager : Singleton<GameManager>
{
	private ulong? _tableId = null;
	private Table _table;
	private string _currentSettings;

	public Table Table
	{
		get
		{
			if (_table == null)
				_table = TableManager.Instance.GetTableById((ulong)_tableId);
			return _table;
		}
		set => _table = value;
	}

	private void Update()
	{
		if (PlayerPrefs.HasKey(StringConstants.SETTINGS_UPDATE) && _currentSettings != PlayerPrefs.GetString(StringConstants.SETTINGS_UPDATE, ""))
		{
			_currentSettings = PlayerPrefs.GetString(StringConstants.SETTINGS_UPDATE, "");
			UserController.Instance.GetUserProfile();
		}
	}


	private void Start()
	{
#if UNITY_STANDALONE

		_currentSettings = PlayerPrefs.GetString(StringConstants.SETTINGS_UPDATE, "");
		StandaloneController.Instance.SetGameSize();

#endif


		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, (handle) =>
		{
			OpenTable();
		});
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserReferenceUpdate, (handle) =>
		{
			OpenTable();
		});
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.TableLoad, (handle) =>
		{
			OpenTable();
		});

		TableManager.OnUpdateRecord.AddListener((table) =>
		{
			it.Logger.Log("TableManager.OnUpdateRecord");
			OpenTable();
		});

	}

	private void OpenTable()
	{

		if (_tableId == null
		|| _table != null
		|| UserController.User == null
		|| UserController.ReferenceData == null
		|| UserController.User.user_profile == null
		|| UserController.User.user_profile.table_theme == null)
		{
			return;
		}
		ConfirmData();
	}

	public void SetTableId(ulong id)
	{
		_tableId = id;

		if (TableManager.Instance.TableList.Count <= 0) return;

		ConfirmData();
	}

	private void ConfirmData()
	{
		it.Logger.Log("ConfirmData");
		if (_table != null)
		{
			return;
		}
		it.Logger.Log("ConfirmData targetId " + _tableId);
		it.Logger.Log("ConfirmData cpunt " + TableManager.Instance.TableList.Count);

		_table = TableManager.Instance.GetTableById((ulong)_tableId);

		if (_table == null)
		{
			it.Logger.Log("ConfirmData null");
		}

		it.Logger.Log("ConfirmData " + _table.id);
		SplashScreen.Instance.LoadComplete();

#if UNITY_STANDALONE

		if(AppConfig.IsLog)
		FileLog.Instance.Init(_table.name + (CommandLineController.GetSession() == null ? "" : $"_session{CommandLineController.GetSession()}"));

		StandaloneController.Instance.AddNewGame(_table);
#endif
	}

	public void SetTable()
	{
		//GameMultiWindowManager.AddNewGame(TableManager.Instance.GetTablebyId(id));

	}


}
