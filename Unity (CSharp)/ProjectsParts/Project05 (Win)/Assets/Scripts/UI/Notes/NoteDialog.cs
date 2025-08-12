using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;
using I2.Loc;

namespace it.UI.Notes
{
  public class NoteDialog : UIDialog
  {
	 [SerializeField]
	 private Text _text;

	 [SerializeField]
	 private Transform _backGround;

	 private string _noteUUid;

	 public string NoteUUid { get => _noteUUid; set => _noteUUid = value; }

	 private void Start()
	 {
		if(_backGround != null)
		_backGround.localScale = new Vector2(1, 0);
	 }

	 public void SetText(string id)
	 {
		_text.text = LocalizationManager.GetTranslation(id);
	 }

	 public void Close()
	 {
		Hide();
	 }
  }
}