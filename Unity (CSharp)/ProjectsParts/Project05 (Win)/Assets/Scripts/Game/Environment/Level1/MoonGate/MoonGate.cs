using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Save;
using UnityEngine;
using Leguar.TotalJSON;
using it.Game.Managers;

namespace it.Game.Environment.Level1.MoonGate
{
  public class MoonGate : Environment, Items.IInteraction, it.Game.Player.Interactions.IInteractionCondition
  {
	 /*
	  * Статусы
	  * 0 - закрыты
	  * 1 - открыто
	  */
	 [System.Serializable]
	 public struct PieceData
	 {
		public GameObject gObject;
		public string uuid;
		public bool isExists;
	 }

	 public PieceData[] _pieces;

	 [SerializeField]
	 private UnityEngine.Events.UnityEvent onAddPiece;

	 public bool IsInteractReady => State == 0;

	 [SerializeField]
	 private Collider _gateCollider;

	 protected override void Start()
	 {
		base.Start();

		_gateCollider.enabled = State <= 0;

	 }

	 protected override void BeforeLoad()
	 {
		base.BeforeLoad();
		for(int i = 0; i < _pieces.Length; i++)
		{
		  _pieces[i].gObject.SetActive(false);
		  _pieces[i].isExists = false;
		}
	 }

	 public override void SubscribeSaveEvents()
	 {
		base.SubscribeSaveEvents();
		Events.EventDispatcher.AddListener(EventsConstants.InventaryGetItem, InventaryGetItem);
	 }

	 public override void UnsubscribeSaveEvents()
	 {
		base.UnsubscribeSaveEvents();
		Events.EventDispatcher.RemoveListener(EventsConstants.InventaryGetItem, InventaryGetItem);
	 }
	 private void InventaryGetItem(com.ootii.Messages.IMessage rMessage)
	 {
		string itemUuid = (string)rMessage.Data;

		//Game.Events.Messages.InventaryAddItem dataIdemGet = (Game.Events.Messages.InventaryAddItem)rMessage;

		bool isPiece = false;

		for (int i = 0; i < _pieces.Length; i++)
		{
		  if (itemUuid.Equals(_pieces[i].uuid))
			 isPiece = true;
		}

		if (!isPiece)
		  return;

		int count = 0;

		for(int i = 0; i < _pieces.Length; i++)
		{
		  count += GameManager.Instance.Inventary.ItemReceived(_pieces[i].uuid) ?
			 1 : 0;		  
		}

		// Показываем диалоги
		if(count == 2)
		{
		  Managers.GameManager.Instance.DialogsManager.ShowDialog("8da6eb77-ce53-4152-a884-94d5810f1118", null,
		  null,
		  null);
		}else if (count == 5)
		{
		  Managers.GameManager.Instance.DialogsManager.ShowDialog("93d5a4a4-2a23-493c-ab67-1a607a73906d", null,
		  null,
		  null);
		}else	if (count > 1)
		{
		  Managers.GameManager.Instance.DialogsManager.ShowDialog("a8c7f0ac-9b62-4bd9-9b48-c5d55a264de5", null,
		  null,
		  null);
		}

	 }

	 /// <summary>
	 /// Событие открытия врат
	 /// </summary>
	 public void OpenGateComplete(bool force = false)
	 {
		Animator animComp = GetComponent<Animator>();
		animComp.SetTrigger((force ? "Force" : "Open"));
	 }

	 public void StartInteract()
	 {
		if (State == 1)
		  return;

		var inventary = Game.Managers.GameManager.Instance.Inventary;

		for (int i = 0; i < _pieces.Length; i++)
		{
		  if (!_pieces[i].isExists)
		  {
			 if (inventary.ExistsItem(_pieces[i].uuid))
			 {
				inventary.Remove(_pieces[i].uuid);
				_pieces[i].isExists = true;
				_pieces[i].gObject.SetActive(true);
				Save();
				onAddPiece?.Invoke();
			 }

		  }
		}


		bool isFull = CheckFull();

		if (isFull)
		{

		  OnComplete();
		}

	 }

	 private void OnComplete()
	 {

		State = 1;
		ConfirmState();
		Save();
		//Managers.GameManager.Instance.DialogsManager.ShowDialog("e0338ec9-4bda-45bc-8793-039ebe9c42a2", null,null,null);
	 }

	 private bool CheckFull(bool force = false)
	 {
		bool full = true;
		foreach (var elem in _pieces)
		{
		  if (!elem.isExists)
			 full = false;
		}

		if (full)
		{
		  OpenGateComplete(force);
		}
		return full;
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		_gateCollider.enabled = State <= 0;

		if(isForce)
		  ForceConfirm();
	 }

	 public void StopInteract()
	 {

	 }

	 private void ForceConfirm()
	 {
		for (int i = 0; i < _pieces.Length; i++)
		{
		  _pieces[i].gObject.SetActive(_pieces[i].isExists);
		}

		CheckFull(true);
	 }

	 public bool InteractionReady()
	 {
		var inventary = Game.Managers.GameManager.Instance.Inventary;
		for (int i = 0; i < _pieces.Length; i++)
		{
		  if (!_pieces[i].isExists)
		  {
			 if (inventary.ExistsItem(_pieces[i].uuid))
			 {
				return true;
			 }

		  }
		}
		return false;
	 }


	 #region Save

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);

		//Dictionary<string, bool> pieses = Newtonsoft.Json.JsonConvert.DeserializeObject < Dictionary<string, bool>>(data.ToString());

		JSON pieses = data as JSON;
		for (int i = 0; i < _pieces.Length; i++)
		{
		  _pieces[i].isExists = pieses.ContainsKey(_pieces[i].uuid) ? pieses.GetBool(_pieces[i].uuid) : false;
		}

	 }

	 protected override JValue SaveData()
	 {
		JSON pieses = new JSON();
		for (int i = 0; i < _pieces.Length; i++)
		{
		  pieses.Add(_pieces[i].uuid, _pieces[i].isExists);
		}

		return pieses;
	 }
	 #endregion

  }
}