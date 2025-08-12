using System.Collections;
using System.Collections.Generic;

using FluffyUnderware.Curvy.Generator.Modules;

using it.Game.Managers;

using UnityEngine;

namespace it.Game.Environment.Handlers
{
  public class AddNote : MonoBehaviour
  {
	 public UnityEngine.Events.UnityEvent _onRead;
	 [SerializeField]
	 private bool _showUi;
	 [SerializeField]
	 private string _noteUuid;

	 public string NoteUuid { get => _noteUuid; set => _noteUuid = value; }

	 private void EventUiDisable(com.ootii.Messages.IMessage Handler)
	 {
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UIDisable, EventUiDisable);
		it.UI.UIDialog dialog = (it.UI.UIDialog)Handler.Data;

		if (!(dialog is it.UI.Notes.NoteDialog))
		  return;

		it.UI.Notes.NoteDialog panel = (it.UI.Notes.NoteDialog)dialog;
		if (panel.NoteUUid == _noteUuid)
		  EmitOnRead();
	 }

	 public void Add()
	 {
		com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.InventaryAddItem, _noteUuid, 0);
		//if(!GameManager.Instance.Inventary.ExistsItem(NoteUuid))
		//  GameManager.Instance.Inventary.AddItem(NoteUuid);
		if (_showUi)
		{
		  com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UIDisable, EventUiDisable);
		  com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.InventaryNote, _noteUuid, 0);
		  //GameManager.Instance.Inventary.GetPrefab(NoteUuid).GetComponent<it.Game.Items.Note>().ShowDialog(EmitOnRead);
		}

	 }

	 [ContextMenu("EmitOnRead")]
	 private void EmitOnRead()
	 {
		_onRead?.Invoke();
	 }
  }
}