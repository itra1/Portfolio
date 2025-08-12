using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sett = it.Settings;

public class LocalizeButton : MonoBehaviour
{
	[SerializeField] private it.UI.LocalizeContextPanel _panel;
	[SerializeField] private string _code;
	[SerializeField] private Image _flag;
	[SerializeField] private TextMeshProUGUI _title;
	private void Start()
	{
 		var lng = Sett.AppSettings.Languages.Find(x => x.Code == _code);
		_title.text = lng.NativeName;
		_flag.sprite = lng.Flag;
		GetComponent<it.UI.Elements.TextButtonUI>().OnClick.AddListener(SelectButton);
	}

	public void SelectButton(){
	if(_panel != null)
		_panel.OpenButton();
		LocalizeController.Instance.SelLocale(_code);
	}

}
