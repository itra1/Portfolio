using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Player.Save;
using it.Game.Items.Inventary;
using it.Game.Items.Symbols;
using it.Game.Managers;
using it.Game.Environment.Handlers;
using it.Game.VideoScenes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Scenes
{
  /// <summary>
  /// Базовый класс для менеджеров уровней
  /// </summary>
  public abstract class LocationManager : SceneBehaviour, ISave
  {
	 public const string EVT_NEW_POSITION = "NEW_POSITION";
	 public const string EVT_START_LEVEL = "START_LEVEL";

	 private UI.Game.GameUI _dialog;

#if UNITY_EDITOR
	 [SerializeField]
	 public string _uuidRestorePoint = "";
#endif

	 [SerializeField]
	 private bool _isSwim = false;
	 public bool IsSwim { get => _isSwim; set => _isSwim = value; }
	 public abstract int LevelIndex { get; }

	 private Light _DayLight;
	 public Light DayLight
	 {
		get
		{
		  if (_DayLight == null)
			 _DayLight = RenderSettings.sun;

		  return _DayLight;
		}
	 }

	 protected override void Start()
	 {
		base.Start();
		SubscribeEvents();
		EmitStart();

		_dialog = UiManager.GetPanel<it.UI.Game.GameUI>();
		_dialog.gameObject.SetActive(true);

	 }

	 private void EmitStart()
	 {
		Game.Events.EventDispatcher.SendMessage(EVT_START_LEVEL, -1);
	 }
	 private void SubscribeEvents()
	 {
		Game.Events.EventDispatcher.AddListener(EVT_NEW_POSITION, (handler) =>
		 {
			var position = handler as Game.Events.Messages.SpawnPosition;

			Game.Events.Messages.SaveData saveData = Game.Events.Messages.SaveData.Allocate();
			saveData.Type = EventsConstants.PlayerProgressSave;
			saveData.Sender = this;
			saveData.Entity = Events.Messages.SaveData.EntityType.levelPosition;
			saveData.SpawnPosition = position.Uuid;
			saveData.SpawnRotetionY = position.RotationY;

			Game.Events.EventDispatcher.SendMessage(saveData);

		 });

		Game.Events.EventDispatcher.AddListener(EventsConstants.InventaryLoad, InventaryLoad);
		Game.Events.EventDispatcher.AddListener(EventsConstants.SymbolLoad, SymbolsLoad);

	 }

	 private void OnDestroy()
	 {
		Game.Events.EventDispatcher.RemoveListener(EventsConstants.InventaryLoad, InventaryLoad);
		Game.Events.EventDispatcher.RemoveListener(EventsConstants.SymbolLoad, SymbolsLoad);

		if (!GameManager.IsQuiting)
		{
		  _dialog = UiManager.GetPanel<it.UI.Game.GameUI>();
		  _dialog.gameObject.SetActive(false);
		}
	 }

	 #region RestorePoints

	 [SerializeField]
	 [Tooltip("Точки восстановления")]
	 protected List<SpawnPoint> _RestorePoints;
	 public List<SpawnPoint> RestorePoints => _RestorePoints;

	 /// <summary>
	 /// Получить точку где восстановиться
	 /// </summary>
	 /// <returns></returns>
	 public Transform RestorePoint
	 {
		get
		{
		  string spawnPosition = null;
		  SpawnPoint point = null;



		  if (GameManager.Instance != null)
			 spawnPosition = GameManager.Instance.UserManager.PlayerProgress.SpawnPosition;

		  if (!string.IsNullOrEmpty(spawnPosition))
		  {
			 point = _RestorePoints.Find(x => x.Uuid.Equals(spawnPosition));

			 if (point != null)
			 {
				return point.SpawnPosition.transform;
			 }

		  }

#if UNITY_EDITOR
		  if (!string.IsNullOrEmpty(_uuidRestorePoint))
		  {
			 point = _RestorePoints.Find(x => x.Uuid.Equals(spawnPosition));
			 if (point != null)
				return point.SpawnPosition.transform;
		  }

#endif

#if UNITY_EDITOR
		  point = _RestorePoints.Find(x => x.Uuid == _uuidRestorePoint);

#else
		  point = _RestorePoints.Find(x => x.IsFirst);

#endif


		  if (point != null)
			 return point.SpawnPosition.transform;
		  else
			 return _RestorePoints[0].SpawnPosition.transform;

		}
	 }

	 [ContextMenu("Find Restore Points")]
	 private void FindAllRestorePoints()
	 {
		_RestorePoints = FindObjectsOfType<SpawnPoint>().ToList();


		//Проверка UUID
		bool isDublicate = false;
		for (int i = 0; i < _RestorePoints.Count; i++)
		{
		  for (int s = 0; s < i; s++)
		  {
			 if (_RestorePoints[i].Uuid == _RestorePoints[s].Uuid)
			 {
				isDublicate = true;
				Logger.LogError(System.String.Format("Деблилование UUID {0} точки восстановление с названием {1}", _RestorePoints[i].Uuid, _RestorePoints[i].name));
			 }
		  }
		}
		if (isDublicate)
		  _RestorePoints.Clear();

	 }

	 #endregion

	 #region Inventary Items

	 [SerializeField]
	 private InventaryItem[] _inventaryItems;

	 public InventaryItem[] InventaryItems { get => _inventaryItems; set => _inventaryItems = value; }


	 private void InventaryLoad(com.ootii.Messages.IMessage rMessage)
	 {
		foreach (var elem in InventaryItems)
		{
		  elem.gameObject.SetActive(!GameManager.Instance.Inventary.ItemReceived(elem.Uuid));
		}
	 }

	 [ContextMenu("Find inventary items")]
	 private void FindAllInventaryItems()
	 {
		InventaryItems = FindObjectsOfType<InventaryItem>();
	 }

	 #endregion

	 #region Symbols

	 [SerializeField]
	 private Symbol[] _symbolsItems;

	 public Symbol[] SymbolsItems { get => _symbolsItems; set => _symbolsItems = value; }


	 private void SymbolsLoad(com.ootii.Messages.IMessage rMessage)
	 {
		foreach (var elem in SymbolsItems)
		{
		  elem.gameObject.SetActive(!GameManager.Instance.Inventary.ItemReceived(elem.Uuid));
		}
	 }

	 [ContextMenu("Find Symbols")]
	 private void FindAllSymbols()
	 {
		SymbolsItems = FindObjectsOfType<Symbol>();
	 }

	 #endregion

	 #region Video scene
	 [SerializeField]
	 private VideoScene[] _videoScenes;
	 public VideoScene[] VideoScenes { get => _videoScenes; set => _videoScenes = value; }

	 [ContextMenu("Video Scenes")]
	 private void FindAllVideoScenes()
	 {
		VideoScenes = FindObjectsOfType<VideoScene>();
	 }
	 #endregion

	 public virtual void StartLevel()
	 {
		var userManager = GameManager.Instance.UserManager;
		var playerProgress = userManager.PlayerProgress;

		userManager.InstantiatePlayer();
		userManager.ResoreUserPosition(RestorePoint);
		GameManager.Instance.GameInputSource.IsEnabled = true;
	 }

#if UNITY_EDITOR

	 [MenuItem("Scene/Location Manager ready")]
	 public static void SceneReady()
	 {
		var lm = MonoBehaviour.FindObjectOfType<LocationManager>();

		if (lm == null)
		{
		  Debug.Log("No location manager");
		  return;
		}

		lm.FindAllInventaryItems();
		lm.FindAllRestorePoints();
		lm.FindAllSymbols();
		lm.FindAllVideoScenes();
	 }

#endif

	 #region Save
	 public virtual void SubscribeSaveEvents()
	 {
		Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void UnsubscribeSaveEvents()
	 {
		Game.Events.EventDispatcher.RemoveListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void LoadHandler(com.ootii.Messages.IMessage handler)
	 {
		Load((handler as Game.Events.Messages.LoadData).SaveData);
	 }

	 public void Load(PlayerProgress progress)
	 {
		//throw new System.NotImplementedException();
	 }

	 public void Save()
	 { }

	 #endregion
  }
}