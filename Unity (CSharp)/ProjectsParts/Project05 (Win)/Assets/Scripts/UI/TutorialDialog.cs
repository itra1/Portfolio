using UnityEngine;
using System.Collections;
using TMPro;

namespace it.UI
{
  public class TutorialDialog : UIDialog
  {
	 [SerializeField] private RectTransform _panel;
	 [SerializeField] private TextMeshProUGUI _title;
	 [SerializeField] private GameObject[] _tutors;

	 private int _index;
	 private FadeColor _fadeColor;
	 private bool isWaitClose;
	 private readonly float _minTimeShow = 1;
	 private float _timeShow;

	 protected override void OnEnable()
	 {
		base.OnEnable();
		_timeShow = Time.time;
		isWaitClose = false;

		if (_fadeColor = null)
		  _fadeColor = GetComponent<FadeColor>();

		if (_fadeColor == null)
		  _fadeColor = gameObject.AddComponent<FadeColor>();

		_fadeColor.Ready(_panel);
		_fadeColor.Show(1);
	 }

	 public void ShowTutor(int index)
	 {
		HideAll();

		switch (index)
		{
		  case 0:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.Move");
			 break;
		  case 1:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.MoveSlowly");
			 break;
		  case 2:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.Interaction");
			 break;
		  case 3:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutirial.Item");
			 break;
		  case 4:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.Symbol");
			 break;
		  case 5:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.Note");
			 break;
		  case 6:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.Jump");
			 break;
		  case 7:
			 _title.text = I2.Loc.LocalizationManager.GetTranslation("Tutorial.Doublejump");
			 break;
		}

		_index = index;
		_tutors[_index].gameObject.SetActive(true);
	 }

	 private void HideAll()
	 {
		for (int i = 0; i < _tutors.Length; i++)
		  _tutors[i].gameObject.SetActive(false);
	 }
	 public void Close()
	 {
		if (_timeShow + _minTimeShow > Time.time)
		  return;
		if (isWaitClose)
		  return;

		isWaitClose = true;

		_fadeColor.Ready(_panel);
		_fadeColor.Hide(1, () =>
		{
		  Hide(0);
		});
	 }

  }
}