using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Save;
using com.ootii.Messages;

namespace it.Game.Managers
{
  public class TutorialManager : Singleton<TutorialManager>, ISave
  {
	 private List<int> _tutorials = new List<int>();

	 private void Start()
	 {
		SubscribeSaveEvents();

		it.Game.Events.EventDispatcher.AddListener(EventsConstants.TutorialRun, RunTutorial);
		it.Game.Events.EventDispatcher.AddListener(EventsConstants.SymbolGetItem, SymbolAdd);
		it.Game.Events.EventDispatcher.AddListener(EventsConstants.InventaryGetItem, InventaryAdd);
		it.Game.Events.EventDispatcher.AddListener(EventsConstants.InteractionFocus, InteractionFocus);

	 }

	 private void OnDestroy()
	 {
		UnsubscribeSaveEvents();
		it.Game.Events.EventDispatcher.RemoveListener(EventsConstants.TutorialRun, RunTutorial);
		it.Game.Events.EventDispatcher.RemoveListener(EventsConstants.SymbolGetItem, SymbolAdd);
		it.Game.Events.EventDispatcher.RemoveListener(EventsConstants.InventaryGetItem, InventaryAdd);
		it.Game.Events.EventDispatcher.RemoveListener(EventsConstants.InteractionFocus, InteractionFocus);
	 }

	 public void RunTutorial(IMessage handler)
	 {
		ShowTutorial((int)handler.Data);
	 }
	 public void SymbolAdd(IMessage handler)
	 {
		ShowTutorial(4);
	 }
	 public void InteractionFocus(IMessage handler)
	 {
		ShowTutorial(2);
	 }
	 public void InventaryAdd(IMessage handler)
	 {
		int index = 3;

		if (_tutorials.Contains(index))
		  return;

		string uuid = (string)handler.Data;

		GameObject pref = it.Game.Managers.GameManager.Instance.Inventary.GetPrefab(uuid);

		var ii = pref.GetComponent<it.Game.Items.Inventary.InventaryItem>();

		if (ii.IsSystem)
		  return;

		ShowTutorial(3);
	 }

	 public void ShowTutorial(int tutorIndex, System.Action onComplete = null)
	 {
		if (_tutorials.Contains(tutorIndex))
		{
		  onComplete?.Invoke();
		  return;
		}

		_tutorials.Add(tutorIndex);
		Save();

		var dialog = UiManager.GetPanel<it.UI.TutorialDialog>();
		dialog.ShowTutor(tutorIndex);
		dialog.gameObject.SetActive(true);
		dialog.onDisable = () =>
		{
		  onComplete?.Invoke();
		  Game.Events.EventDispatcher.SendMessage(this, EventsConstants.TutorialComplete, tutorIndex, 0f);
		};
	 }

	 public void SubscribeSaveEvents()
	 {
		it.Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void UnsubscribeSaveEvents()
	 {
		it.Game.Events.EventDispatcher.RemoveListener(EventsConstants.PlayerProgressLoad, LoadHandler);
	 }

	 public void LoadHandler(IMessage handler)
	 {
		Load((handler as Game.Events.Messages.LoadData).SaveData);
	 }

	 public void Load(PlayerProgress progress)
	 {
		_tutorials.Clear();

		for (int i = 0; i < progress.Tutorial.Length; i++)
		{
		  _tutorials.Add(progress.Tutorial.GetInt(i));
		}
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
		saveData.Entity = Events.Messages.SaveData.EntityType.tutorial;

		for (int i = 0; i < _tutorials.Count; i++)
		{
		  saveData.Tutorial.Add(_tutorials[i]);
		}
		Game.Events.EventDispatcher.SendMessage(saveData);
	 }
  }
}