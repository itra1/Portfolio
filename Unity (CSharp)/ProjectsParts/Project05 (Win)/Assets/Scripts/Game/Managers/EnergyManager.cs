using UnityEngine;
using System.Collections;
using com.ootii.Messages;
using it.Game.Player.Save;
using QFSW.QC;

namespace it.Game.Managers
{
  public class EnergyManager : MonoBehaviourBase
  {
	 public const string EVT_LOAD = "ENERGY_LOAD";
	 public const string EVT_ENERGY_CHANGE = "ENERGY_CHANGE";

	 private float _maxValue = 100;
	 public float Percent => _value / _maxValue;

	 private float _value;

	 public float Value => _value;

	 private void Start()
	 {
		SubscribeSaveEvents();
		Load(Game.Managers.GameManager.Instance.UserManager.PlayerProgress);
	 }

	 private void OnDestroy()
	 {
		UnsubscribeSaveEvents();
	 }
	 public void Add(float increment)
	 {
		_value += increment;
		_value = Mathf.Clamp(_value, 0, _maxValue);
		EmitEnergyChange();
		Save();
	 }

	 [Command]
	 public void SetEnegry(float targetValue)
	 {
		_value = targetValue;
		_value = Mathf.Clamp(_value, 0, _maxValue);
		EmitEnergyChange();
		Save();
	 }

	 public void Subtract(float decriment)
	 {
		_value -= decriment;
		_value = Mathf.Clamp(_value, 0, _maxValue);
		EmitEnergyChange();
		Save();
	 }


	 private void EmitEnergyChange()
	 {
		Game.Events.EventDispatcher.SendMessage(EVT_ENERGY_CHANGE);
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
		_value = progress.Energy;
		EmitEnergyChange();

		Game.Events.EventDispatcher.SendMessage(EVT_LOAD);
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
		saveData.Entity = Events.Messages.SaveData.EntityType.energy;

		saveData.Energy = Value;

		Game.Events.EventDispatcher.SendMessage(saveData);
	 }

	 #endregion

  }
}