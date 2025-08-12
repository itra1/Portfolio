using UnityEngine;
using System.Collections;
using it.Game.Items.Inventary;
using it.Game.Managers;

namespace it.Game.Items
{
  public class Note : InventaryItem
  {
	 [SerializeField]
	 private string _langId;

	 public string LangId { get => _langId; set => _langId = value; }

	 protected override void OnEnable()
	 {
		CheckExistsAndDeactive = false;
		base.OnEnable();
	 }

	 public virtual void ShowDialog()
	 {
		ShowDialog(null);
	 }
	 public virtual void ShowDialog(UnityEngine.Events.UnityAction onClose)
	 {

		var panel = UiManager.GetPanel<it.UI.Notes.NoteDialog>();
		panel.NoteUUid = Uuid;
		panel.SetText(LangId);
		panel.gameObject.SetActive(true);
		GameManager.Instance.SetCursorVisible(this, true);
		panel.onDisable = () =>
		{
		  GameManager.Instance.SetCursorVisible(this, false);
		  onClose?.Invoke();
		};
	 }

  }
}