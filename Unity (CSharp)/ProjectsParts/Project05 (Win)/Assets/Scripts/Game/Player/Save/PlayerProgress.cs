using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leguar.TotalJSON;

namespace it.Game.Player.Save
{
  /// <summary>
  /// Прогресс пользователя
  /// </summary>
  [CreateAssetMenu(fileName = "PlayerSave", menuName = "ScriptableObject/PlayerSave", order = 0)]
  public class PlayerProgress : ScriptableObject
  {
	 /// <summary>
	 /// Загружен
	 /// </summary>
	 public bool IsLoad { get => _isLoad; private set => _isLoad = value; }
	 /// <summary>
	 /// Текущий уровень
	 /// </summary>
	 public int Level { get => _level; private set => _level = value; }
	 /// <summary>
	 /// Позиция на уровне
	 /// </summary>
	 public string SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }
	 public float SpawnRotationY { get => _spawnRotationY; set => _spawnRotationY = value; }
	 /// <summary>
	 /// Данные по объектам на уровне
	 /// </summary>
	 public JSON Environment { get => _environment; private set => _environment = value; }
	 /// <summary>
	 /// Инвентарь
	 /// </summary>
	 public JSON Inventary { get => _inventary; set => _inventary = value; }
	 /// <summary>
	 /// Символы
	 /// </summary>
	 public JArray Symbols { get => _symbols; set => _symbols = value; }
	 /// <summary>
	 /// Туториал
	 /// </summary>
	 public JArray Tutorial { get => _tutorial; set => _tutorial = value; }
	 private List<JSON> SaveList { get => _saveList; set => _saveList = value; }

	 private bool _isLoad = false;

	 [SerializeField]
	 private int _level = 0;

	 private float _energy;
	 public float Energy { get => _energy; set => _energy = value; }

	 [SerializeField]
	 private string _spawnPosition = "";
	 [SerializeField]
	 private float _spawnRotationY = 0;

	 [SerializeField]
	 private JSON _environment = new JSON();

	 [SerializeField]
	 private JSON _inventary = new JSON();

	 private JArray _symbols = new JArray();

	 private List<JSON> _saveList = new List<JSON>();

	 private JArray _tutorial = new JArray();

	 public JValue GetEnvironment(string uuid)
	 {
		if (!Environment.ContainsKey(uuid))
		  return null;
		return Environment.Get(uuid);
	 }

	 public void SaveData(Game.Events.Messages.SaveData saveData)
	 {

		bool save = false;

		switch (saveData.Entity)
		{
		  case Events.Messages.SaveData.EntityType.environment:

			 foreach (var elem in saveData.Environment)
			 {

				if (!Environment.ContainsKey(elem.Key))
				  Environment.Add(elem.Key, null);
				Environment[elem.Key] = elem.Value;
			 }

			 break;
		  case Events.Messages.SaveData.EntityType.levelPosition:
			 if (!string.IsNullOrEmpty(saveData.SpawnPosition))
			 {
				SpawnPosition = saveData.SpawnPosition;
				SpawnRotationY = saveData.SpawnRotetionY;
				save = true;
			 }
			 break;
		  case Events.Messages.SaveData.EntityType.energy:
			 Energy = saveData.Energy;
			 break;
		  case Events.Messages.SaveData.EntityType.inventary:
			 Inventary = saveData.Inventary;
			 break;
		  case Events.Messages.SaveData.EntityType.symbol:
			 Symbols = saveData.Symbols;
			 break;
		  case Events.Messages.SaveData.EntityType.tutorial:
			 Tutorial = saveData.Tutorial;
			 break;
		  case Events.Messages.SaveData.EntityType.none:
		  default:
			 break;
		}

		if (save)
		  Save();
	 }

	 /// <summary>
	 /// УСтановка значения первого запуска
	 /// </summary>
	 private void SetFirstStart()
	 {
		ClearSaveData();
		Save();
	 }

	 public void FullClear()
	 {
		ClearSaveData();
		SaveList.Clear();
	 }

	 private void ClearSaveData()
	 {
		Level = 0;
		SpawnPosition = "";
		Environment.Clear();
		Inventary.Clear();
		Symbols.Clear();
	 }

	 public void SetLevel(int level)
	 {
		bool save = this.Level != 0 && this.Level != level;

		this.Level = level;

		if (save)
		  Save();
	 }

	 /// <summary>
	 /// Загрузка прогресса
	 /// </summary>
	 /// <param name="index">Индеч в текущей очереди</param>
	 public void Load(int index = -1)
	 {
		ClearSaveData();
		//Если список не загружен, грузим
		if (SaveList.Count == 0)
		{
		  string saveList = PlayerPrefs.GetString("SaveList", "");

		  if (string.IsNullOrEmpty(saveList))
		  {
			 IsLoad = true;
			 SetFirstStart();
			 SendLoadMessage();
			 Game.Logger.Log("Loading default complete");
			 return;
		  }

		  SaveList = JArray.ParseString(saveList).AsJSONArray().ToList();
		  SaveList = SaveList.OrderBy(x => x.GetInt("index")).ToList();
		}

		//Загружаем последнее сохранение из списка
		string saveDataString = PlayerPrefs.GetString(index == -1 ? SaveList.Last().GetString("uuid") : SaveList[index].GetString("uuid"), "");

		Debug.Log("Load: " + saveDataString);

		JSON saveData = JSON.ParseString(saveDataString, "saveData");

		if (saveData.ContainsKey("Level"))
		  Level = saveData.GetInt("Level");
		else
		  Level = 0;

		if (saveData.ContainsKey("Energy"))
		  Energy = saveData.GetFloat("Energy");
		else
		  Energy = 0;

		if (saveData.ContainsKey("SpawnPosition") && saveData["SpawnPosition"] != null)
		  SpawnPosition = saveData.GetString("SpawnPosition");
		else
		  SpawnPosition = null;

		if (saveData.ContainsKey("SpawnRotationY") && saveData["SpawnRotationY"] != null)
		  SpawnRotationY = saveData.GetFloat("SpawnRotationY");
		else
		  SpawnRotationY = 0;

		if (saveData.ContainsKey("Environment"))
		  Environment = saveData.GetJSON("Environment");
		else
		  Environment.Clear();

		if (saveData.ContainsKey("Inventary"))
		  Inventary = saveData.GetJSON("Inventary");
		else
		  Inventary.Clear();

		if (saveData.ContainsKey("Symbols"))
		  Symbols = saveData.GetJArray("Symbols");
		else
		  Symbols.Clear();

		IsLoad = true;

		SendLoadMessage();

		Game.Logger.Log("Loading complete");
	 }
	 /// <summary>
	 /// Сохраняем
	 /// </summary>
	 public void Save()
	 {

		if (!IsLoad)
		  return;

		JSON saveStruct = new JSON();
		saveStruct.Add("date", System.DateTime.Now.ToString());
		saveStruct.Add("uuid", System.Guid.NewGuid().ToString());
		saveStruct.Add("index", GetSaveIndex());

		// Удаляем самоее раннее сохранение, если превышен лимит
		if (SaveList.Count > ProjectSettings.MaxSaveToUser - 1)
		{
		  string uuidRemove = SaveList[0].GetString("uuid");
		  SaveList.RemoveAt(0);
		  PlayerPrefs.DeleteKey(uuidRemove);
		}

		SaveList.Add(saveStruct);

		PlayerPrefs.SetString("SaveList", (new JArray(SaveList)).CreateString());

		JSON saveData = new JSON();
		saveData.Add("Level", Level);
		saveData.Add("Energy", Energy);
		saveData.Add("SpawnPosition", SpawnPosition);
		saveData.Add("SpawnRotationY", SpawnRotationY);
		saveData.Add("Environment", Environment);
		saveData.Add("Inventary", Inventary);
		saveData.Add("Symbols", Symbols);

		string saveString = saveData.CreateString();
#if UNITY_EDITOR
		Debug.Log("Save: " + saveString);
#endif

		PlayerPrefs.SetString(saveStruct.GetString("uuid"), saveString);
		PlayerPrefs.Save();

	 }

	 /// <summary>
	 /// Получение индекса для нового сохранения
	 /// </summary>
	 /// <returns></returns>
	 private long GetSaveIndex()
	 {
		return SaveList.Count == 0
		  ? 1
		  : SaveList[SaveList.Count - 1].GetInt("index") + 1;
	 }

	 private void SendLoadMessage()
	 {
		//Game.Events.EventDispatcher.SendMessage(this, EventsConstants.PlayerProgressLoad, this, 0.1f);

		Game.Events.Messages.LoadData loadData = Events.Messages.LoadData.Allocate();
		loadData.Type = EventsConstants.PlayerProgressLoad;
		loadData.Sender = this;
		loadData.SaveData = this;
		loadData.Delay = 0.1f;
		Game.Events.EventDispatcher.SendMessage(loadData);
	 }

	 private void SendSaveMessage()
	 {
		Game.Events.Messages.SaveData saveData = Events.Messages.SaveData.Allocate();
		saveData.Type = EventsConstants.PlayerProgressSave;
		saveData.Sender = this;
		saveData.Entity = Events.Messages.SaveData.EntityType.none;
		saveData.Delay = 0f;
		Game.Events.EventDispatcher.SendMessage(saveData);
	 }

	 //private struct SaveStruct
	 //{
	 //// Дата сохранения
	 //public string date;
	 //// UUID сохранения
	 //public string uuid;
	 //// Индекс сохранения
	 //public long index;
	 //}

  }

}