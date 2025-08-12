using UnityEngine;
using TMPro;

namespace it.UI.Settings
{
  [System.Serializable]
  public class SelectEvent : UnityEngine.Events.UnityEvent<int> { }
  public class SwitchItem : Item
  {
	 public UnityEngine.Events.UnityAction<int> _selectEvent;
	 [SerializeField]
	 private string[] _values;
	 [SerializeField]
	 private TextMeshProUGUI _textValue;

	 [SerializeField]
	 private bool _loop = true;

	 private int _index;

	 public int Index { get => _index; set => _index = value; }

	 protected override void OnEnable()
	 {
		base.OnEnable();
		SetText();
	 }

	 public void NextButton()
	 {
		int index = _index;
		index++;

		if (_loop)
		{
		  if (index >= _values.Length)
			 index = 0;
		}

		index = Mathf.Clamp(index, 0, _values.Length - 1);

		SetIndex(index);
	 }

	 public void BackButton()
	 {
		int index = _index;
		index--;

		if (_loop)
		{
		  if (index < 0)
			 index = _values.Length-1;
		}

		index = Mathf.Clamp(index, 0, _values.Length - 1);

		SetIndex(index);
	 }

	 public void SetIndex(int index)
	 {
		_index = index;
		SetText();
		ChangeEmit();
	 }

	 private void ChangeEmit()
	 {
		_selectEvent?.Invoke(_index);
	 }

	 private void SetText()
	 {
		var loc = _textValue.GetComponent<I2.Loc.Localize>();
		loc.Term = _values[_index];
		//_textValue.text = I2.Loc.LocalizationManager.GetTermTranslation(_values[_index]);
	 }

  }
}