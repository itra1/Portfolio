using System.Collections;
using System.Collections.Generic;
using com.ootii.Messages;
using it.Game.Player.Save;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Items.Inventary
{

#if UNITY_EDITOR
  [CustomEditor(typeof(InventaryManager))]
  public class InventaryManagerEditor: Editor
  {
	 private string _targetUuid;
	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		_targetUuid = EditorGUILayout.TextField("UUID", _targetUuid);

		if (GUILayout.Button("Add"))
		{
		  if(!string.IsNullOrEmpty(_targetUuid) && !string.IsNullOrWhiteSpace(_targetUuid))
			 ((InventaryManager)target).FakeAddItem(_targetUuid);
		}

	 }
  }
#endif

  /// <summary>
  /// Инвентарь
  /// </summary>
  public class InventaryManager : MonoBehaviourBase, ISave, IActionOrder
  {

	 private InventaryLibrary _library;
	 /// <summary>
	 /// Данные
	 /// </summary>
	 private Dictionary<string, int> _data = new Dictionary<string, int>();

	 private Dictionary<string, int> Data
	 {
		get
		{
		  return _data;
		}
		set
		{
		  _data = value;
		}
	 }

	 public int ActionOrderID { get; set; }

	 private Dictionary<string, GameObject> _dataPrefabs = new Dictionary<string, GameObject>();

	 private void Start()
	 {
		SubscribeSaveEvents();
		_library = Resources.Load<InventaryLibrary>(Game.ProjectSettings.InventaryLibrary);
		Load(Game.Managers.GameManager.Instance.UserManager.PlayerProgress);

	 }

	 private void OnEnable()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.InventaryAddItem, AddItemEvent);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.InventaryNote, ShowNote);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.InventaryOpenPanel, OpenInventaryPanel);
	 }

	 private void OnDisable()
	 {
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.InventaryAddItem, AddItemEvent);
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.InventaryNote, ShowNote);
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.InventaryOpenPanel, OpenInventaryPanel);
	 }

	 private void OnDestroy()
	 {
		UnsubscribeSaveEvents();
	 }

	 private void AddItemEvent(com.ootii.Messages.IMessage hander)
	 {
		string newUuid = (string)hander.Data;

		if (!ExistsItem(newUuid))
		  AddItem(newUuid);
		//if (!GameManager.Instance.Inventary.ExistsItem(NoteUuid))
		//  GameManager.Instance.Inventary.AddItem(NoteUuid);
	 }

	 private void OpenInventaryPanel(IMessage hander)
	 {
		it.UI.Inventary.InventaryPanel.ShowPanel();
	 }

	 #region Note

	 private string _noteDialog;
	 private void ShowNote(com.ootii.Messages.IMessage hander)
	 {
		_noteDialog = (string)hander.Data;
		MessageDispatcher.SendMessage(this, EventsConstants.ActionsOrderAdd, this, 0);
	 }

	 public void ActionOrder()
	 {
		GetPrefab(_noteDialog).GetComponent<Note>().ShowDialog(()=> {
		  MessageDispatcher.SendMessage(this, EventsConstants.ActionsOrderRemove, ActionOrderID, 0);
		});
	 }

	 #endregion

	 //private bool _showPanel = false;

	 //public void ShowPanel()
	 //{
		//  //var panelSymbol = it.Game.Managers.UiManager.GetPanel<it.UI.Symbols.SymbolWall>();

		//  //if (panelSymbol.gameObject.activeInHierarchy)
		//	 //return;
		
		//_showPanel = !_showPanel;

		//var panel = it.Game.Managers.UiManager.GetPanel<it.UI.Inventary.Inventary>();

		//panel.onEnamble = () =>
		//{
		//  it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = false;
		//};
		//panel.onDisable = () =>
		//{
		//  it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = true;
		//};
		//if (_showPanel)
		//  panel.Show();
		//else
		//  panel.Hide();
	 //}

	 /// <summary>
	 /// Добвление предмета
	 /// </summary>
	 /// <param name="uuid"></param>
	 /// <param name="count"></param>
	 public void AddItem(string uuid, int count = 1)
	 {
		if (Data.ContainsKey(uuid))
		{
		  Data[uuid] += count;
		}
		else
		{
		  Data.Add(uuid, count);
		}
		Save();
		com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.InventaryGetItem, uuid, 0);
		//SendMessageItemInventary(uuid, count);
	 }

	 //private void SendMessageItemInventary(string uuid, int count)
	 //{
		//Game.Events.Messages.InventaryAddItem saveData = Events.Messages.InventaryAddItem.Allocate();
		//saveData.Type = EVT_GET_ITEM;
		//saveData.Sender = this;
		//saveData.Uuid = uuid;
		//saveData.Count = count;

		//Game.Events.EventDispatcher.SendMessage(saveData);
	 //}

	 /// <summary>
	 /// Получение количества
	 /// </summary>
	 /// <param name="uuid"></param>
	 /// <returns></returns>
	 public int GetCount(string uuid)
	 {
		return !Data.ContainsKey(uuid) ? 0 : 1;
	 }

	 public bool ExistsItem(string uuid)
	 {
		return Data.ContainsKey(uuid) && Data[uuid] > 0;
	 }

	 public bool ItemReceived(string uuid)
	 {
		return Data.ContainsKey(uuid);
	 }

	 

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

		foreach(var elem in Data)
		{
		  if(elem.Value > 0)
			 prefabs.Add(GetPrefab(elem.Key));
		}
		return prefabs;
	 }

	 /// <summary>
	 /// Загрузка префаба из ресурсов
	 /// </summary>
	 /// <param name="uuid"></param>
	 private void LoadPrefab(string uuid)
	 {
		Debug.Log(_library.GetPath(uuid));

		_dataPrefabs.Add(uuid, Resources.Load<GameObject>(_library.GetPath(uuid)));
	 }

	 public void Remove(string uuid)
	 {
		Data[uuid] -= 1;
		Save();
	 }

	 /// <summary>
	 /// Очистка данных
	 /// </summary>
	 private void ClearData()
	 {
		Data.Clear();
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
		foreach (var key in progress.Inventary.Keys)
		{
		  _data.Add(key, progress.Inventary.GetInt(key));
		}
		Game.Events.EventDispatcher.SendMessage(EventsConstants.InventaryLoad);
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
		saveData.Entity = Events.Messages.SaveData.EntityType.inventary;

		foreach (var elem in _data)
		{
		  saveData.Inventary.Add(elem.Key, elem.Value);
		}

		Game.Events.EventDispatcher.SendMessage(saveData);
	 }
	 #endregion

#if UNITY_EDITOR

	 public void FakeAddItem(string uuid)
	 {
		AddItem(uuid, 1);
	 }


#endif

  }

}