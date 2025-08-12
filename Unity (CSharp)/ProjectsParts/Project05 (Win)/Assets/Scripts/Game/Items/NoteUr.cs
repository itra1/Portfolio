using UnityEngine;
using System.Collections;
using it.Game.Items.Inventary;
using it.Game.Managers;

namespace it.Game.Items
{
  public class NoteUr : Note
  {

	 public override void ShowDialog(UnityEngine.Events.UnityAction onClose)
	 {

		var panel = UiManager.GetPanel<it.UI.Notes.UrDialog>();
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