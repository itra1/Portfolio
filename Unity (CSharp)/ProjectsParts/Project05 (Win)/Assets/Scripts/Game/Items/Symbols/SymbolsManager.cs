using System.Collections;
using System.Collections.Generic;
using com.ootii.Messages;
using it.Game.Player.Save;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Items.Symbols
{
  public class SymbolsManager : MonoBehaviourBase, ISave, IActionOrder
  {

	 private SymbolsLibrary _library;

	 private List<string> _data = new List<string>();

	 private Dictionary<string, GameObject> _dataPrefabs = new Dictionary<string, GameObject>();

	 private string _newUUID = "";

	 public string NewUUID { get => _newUUID; set => _newUUID = value; }
	 public int ActionOrderID { get; set; }

	 private void Start()
	 {
		SubscribeSaveEvents();
		_library = Resources.Load<SymbolsLibrary>(Game.ProjectSettings.SymbolsLibrary);
		Load(Game.Managers.GameManager.Instance.UserManager.PlayerProgress);
	 }

	 private void OnDestroy()
	 {
		UnsubscribeSaveEvents();
	 }

	 private void OnEnable()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SymbolOpenPanel, OpenPanel);
	 }

	 private void OnDisable()
	 {
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SymbolOpenPanel, OpenPanel);
	 }
	 private void OpenPanel(IMessage hander)
	 {
		ShowPanel();
	 }

	 /// <summary>
	 /// Добвление предмета
	 /// </summary>
	 /// <param name="uuid"></param>
	 /// <param name="count"></param>
	 public void AddItem(string uuid)
	 {
		if (!_data.Contains(uuid))
		{
		  _data.Add(uuid);
		}
		//AddNewVisibleSymbol(uuid);
		_newUUID = uuid;
		Save();
		MessageDispatcher.SendMessage(this, EventsConstants.SymbolGetItem, uuid, 0.2f);
	 }

	 private void AddNewVisibleSymbol(string uuid)
	 {
		_newUUID = uuid;

		//MessageDispatcher.SendMessage(this, EventsConstants.ActionsOrderAdd, this, 0);

		//DOVirtual.DelayedCall(0.5f, VisibleDialog);
	 }

	 public void ActionOrder()
	 {
		MessageDispatcher.SendMessage(this, EventsConstants.ActionsOrderRemove, ActionOrderID, 0.1f);
	 }

	 public bool ExistsItem(string uuid)
	 {
		return _data.Contains(uuid);
	 }

	 private bool _showPanel = false;
	 public void ShowPanel()
	 {
		it.UI.Symbols.SymbolWallPanel.ShowPanel();


		//_showPanel = !_showPanel;

		//it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
		//	  new Color32(0, 0, 0, 255), 0.5f, null, () =>
		//	  {
		//		 VisibleDialog(_showPanel);
		//	  }, () =>
		//	  {

		//	  });
	 }

	 //public void VisibleDialog(bool isVisible)
	 //{
		//var panel = it.Game.Managers.UiManager.GetPanel<it.UI.Symbols.SymbolWall>();
		//panel.gameObject.SetActive(isVisible);
	 //}


	 /// <summary>
	 /// Получение префаба
	 /// </summary>
	 /// <param name="guid"></param>
	 /// <returns></returns>
	 public GameObject GetPrefab(string guid)
	 {
		if (!_dataPrefabs.ContainsKey(guid))
		  LoadPrefab(guid);

		return _dataPrefabs[guid];
	 }

	 public List<GameObject> GetAllPrefabs()
	 {
		List<GameObject> prefabs = new List<GameObject>();

		foreach (var elem in _data)
		  prefabs.Add(GetPrefab(elem));
		return prefabs;
	 }

	 /// <summary>
	 /// Загрузка префаба из ресурсов
	 /// </summary>
	 /// <param name="uuid"></param>
	 private void LoadPrefab(string uuid)
	 {
		_dataPrefabs.Add(uuid, Resources.Load<GameObject>(_library.GetPath(uuid)));
	 }

	 public void Remove(string uuid)
	 {
		_data.Remove(uuid);
		Save();
	 }

	 /// <summary>
	 /// Очистка данных
	 /// </summary>
	 private void ClearData()
	 {
		_data.Clear();
		_dataPrefabs.Clear();
	 }

	 #region Save
	 public void SubscribeSaveEvents()
	 {
		Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void UnsubscribeSaveEvents()
	 {
		Game.Events.EventDispatcher.RemoveListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void LoadHandler(IMessage handler)
	 {
		Load((handler as Game.Events.Messages.LoadData).SaveData);
	 }

	 public void Load(PlayerProgress progress)
	 {
		ClearData();
		string[] loadData = progress.Symbols.AsStringArray();
		foreach (var elem in loadData)
		{
		  _data.Add(elem);
		}
		Game.Events.EventDispatcher.SendMessage(EventsConstants.SymbolLoad);
	 }

	 public void Save()
	 {

		SendSaveMessage();
	 }

	 private void SendSaveMessage()
	 {
		Game.Events.Messages.SaveData saveData = Events.Messages.SaveData.Allocate();
		saveData.Type = EventsConstants.PlayerProgressSave;
		saveData.Sender = this;
		saveData.Entity = Events.Messages.SaveData.EntityType.symbol;

		foreach (var elem in _data)
		{
		  saveData.Symbols.Add(elem);
		}

		Game.Events.EventDispatcher.SendMessage(saveData);
	 }

	 #endregion

  }
}