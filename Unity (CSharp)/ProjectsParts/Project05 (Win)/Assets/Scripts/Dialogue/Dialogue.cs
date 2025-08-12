using UnityEngine;
using System.Collections;
using it.Game.Managers;

namespace it.Dialogue
{


  public class Dialogue: UUIDBase
  {
	 [SerializeField]
	 private int _level;
	 [SerializeField]
	 private string _descriptions;

	 public DialogueItem[] _items;

	 public UnityEngine.Events.UnityAction onStart;
	 public UnityEngine.Events.UnityAction<int> onNextFrame;
	 public UnityEngine.Events.UnityAction onComplete;

	 public void Show(UnityEngine.Events.UnityAction onStart, UnityEngine.Events.UnityAction<int> onNextFrame, UnityEngine.Events.UnityAction onComplete)
	 {
		this.onStart = onStart;
		this.onNextFrame = onNextFrame;
		this.onComplete = onComplete;

		it.UI.Dialogue.Dialogue dialogPanel = UiManager.GetPanel<it.UI.Dialogue.Dialogue>();

		dialogPanel.gameObject.SetActive(true);
		dialogPanel.Show(this);

	 }

  }
}