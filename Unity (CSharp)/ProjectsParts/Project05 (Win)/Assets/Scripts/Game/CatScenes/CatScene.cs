using System.Collections;
using System.Collections.Generic;
using com.ootii.Messages;
using it.Game.Player.Save;
using Leguar.TotalJSON;

namespace it.Game.CatScenes
{
  /// <summary>
  /// Кат сцена
  /// </summary>
  public abstract class CatScene : UUIDBase, ISave
  {
	 /// <summary>
	 /// Разрешение на работу кат сцен
	 /// </summary>
	 public static bool IsGranted = false;

	 /*
	  * Состояния:
	  * 
	  * 0 - закрта
	  * 1 - открыта
	  * 2 - активна
	  * 3 - окончена
	  */
	 private int _state = 0;
	 public int State
	 {
		get => _state; set
		{
		  if (_state == value)
			 return;

		  _state = value;
		  ConfirmState();
		}
	 }


	 /// <summary>
	 /// Номер кат сцены
	 /// </summary>
	 public abstract int Number { get; }
	 protected bool IsLoading { get => _isLoading; private set => _isLoading = value; }

	 private bool _isLoading;
	 protected virtual void Awake()
	 {
	 }

	 protected virtual void Start()
	 {
		Load(Game.Managers.GameManager.Instance.UserManager.PlayerProgress);
		SubscribeSaveEvents();
	 }



	 protected virtual void OnDestroy()
	 {
		UnsubscribeSaveEvents();
	 }
	 /// <summary>
	 /// Зфпуск
	 /// </summary>
	 public virtual void Play()
	 {
		if (!IsGranted || State != 1)
		  return;
		SetPlay();
	 }
	 /// <summary>
	 /// Остановка
	 /// </summary>
	 public virtual void Stop()
	 {
		SetStop();
	 }
	 /// <summary>
	 /// Готов к запуску
	 /// </summary>
	 public virtual void Ready()
	 {
		if (!IsGranted || State != 1)
		  return;

		SetReady();
	 }

	 public virtual void SetReady()
	 {
		State = 1;
		SendStateChangeMessage();
		Save();
	 }
	 /// <summary>
	 /// запусе
	 /// </summary>
	 public virtual void SetPlay()
	 {
		State = 2;

		Game.Events.EventDispatcher.AddListener(Game.Player.PlayerBehaviour.EVT_PLAYER_ANIM_NAME, PlayerEventAnim);

		SendStateChangeMessage();
		Save();
	 }

	 /// <summary>
	 /// Остановка
	 /// </summary>
	 public virtual void SetStop()
	 {
		State = 3;
		Game.Events.EventDispatcher.RemoveListener(Game.Player.PlayerBehaviour.EVT_PLAYER_ANIM_NAME, PlayerEventAnim);

		SendStateChangeMessage();
		Save();
	 }
	 public virtual void PlayerEventAnim(com.ootii.Messages.IMessage handler)
	 {
		if (State != 2)
		  return;

		var data = handler as Game.Events.Messages.Player.AnimEvent;

		if (data.AnimEventName == Game.Player.PlayerBehaviour.ANIM_EVENT_STOP_CATSCENE)
		{
		  SetStop();
		}

	 }

	 private void SendStateChangeMessage()
	 {

		Game.Events.Messages.CatScene eventData = Events.Messages.CatScene.Allocate();
		eventData.Type = EventsConstants.PlayerProgressSave;
		eventData.Sender = this;

		switch (State)
		{
		  case 1:
			 eventData.State = Events.Messages.CatScene.StateType.ready;
			 break;
		  case 2:
			 eventData.State = Events.Messages.CatScene.StateType.start;
			 break;
		  case 3:
			 eventData.State = Events.Messages.CatScene.StateType.complete;
			 break;
		}

		eventData.Uuid = Uuid;

		Game.Events.EventDispatcher.SendMessage(eventData);
	 }

	 protected virtual void ConfirmState()
	 {
		if (!IsLoading)
		  Save();
	 }

	 #region Save
	 public void Load(PlayerProgress progress)
	 {
		if (!progress.IsLoad)
		  return;

		IsLoading = true;
		JValue data = progress.GetEnvironment(Uuid);

		if (data == null)
		  return;

		//Dictionary<string, object> loadData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ToString());
		JSON loadData = data as JSON;// JsonUtility.FromJson<Dictionary<string, JValue>>(data.ToString());

		State = loadData.GetInt("state");
		LoadData(loadData.Get("data"));

		IsLoading = false;
	 }

	 protected virtual void LoadData(JValue data) { }

	 public void LoadHandler(IMessage handler)
	 {
		Load((handler as Game.Events.Messages.LoadData).SaveData);
	 }

	 public virtual void SubscribeSaveEvents()
	 {
		Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public virtual void UnsubscribeSaveEvents()
	 {
		Game.Events.EventDispatcher.RemoveListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void Save()
	 {

		JSON saveData = new JSON();
		saveData.Add("state", State);
		saveData.Add("data", SaveData());

		SendSaveMessage(saveData);
	 }
	 public virtual JValue SaveData()
	 {
		return new JSON();
	 }

	 private void SendSaveMessage(JValue data)
	 {

		Game.Events.Messages.SaveData saveEvent = Events.Messages.SaveData.Allocate();
		saveEvent.Type = EventsConstants.PlayerProgressSave;
		saveEvent.Sender = this;
		saveEvent.Entity = Events.Messages.SaveData.EntityType.environment;
		saveEvent.Environment.Add(Uuid, data);

		Game.Events.EventDispatcher.SendMessage(saveEvent);
	 }

	 #endregion


  }
}