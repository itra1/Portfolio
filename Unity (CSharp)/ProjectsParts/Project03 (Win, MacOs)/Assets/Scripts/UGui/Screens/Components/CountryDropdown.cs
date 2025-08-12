using Providers.Network.Materials;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UGui.Screens.Components
{
	public class CountryDropdown : MonoBehaviour
	{
		[HideInInspector] public UnityEngine.Events.UnityEvent<string> onChangeValue = new();

		[SerializeField] private TMP_InputField _resultField;
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private RectTransform _dropDownpanel;
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private Button _buttonItemPrefab;
		[SerializeField] private int _maxItems;
		[SerializeField] private RectTransform _arrowRect;

		private PoolList<Button> _listItems;
		private List<CountryData> _countries = new();
		private bool _listVisible;
		private string _searchValue = "";
		private CountryData _selectedElement;
		public List<CountryData> Options { get => _countries; set => _countries = value; }
		public int Value => Options.FindIndex(x => x.title == _inputField.text);

		public void OnEnable()
		{
			Clear();
			_listItems ??= new PoolList<Button>(_buttonItemPrefab.gameObject, _scrollRect.content);
			_buttonItemPrefab.gameObject.SetActive(false);
			_dropDownpanel.gameObject.SetActive(false);

			//_scrollRect = GetComponentInChildren<ScrollRect>();
			//_scrollRect.gameObject.SetActive(false);
			//_arrowRect.localScale = new Vector3(1, 1, 1);
			_scrollRect.vertical = true;
			_inputField.onValueChanged.RemoveAllListeners();
			_inputField.onValueChanged.AddListener((string value) =>
			{
				_searchValue = value;
				ShowCompareOptions(_searchValue);
			});
		}

		public CountryData GetValue(){
			return _selectedElement;
		}

		public void VisibleChangeButtonTouch(){
			_listVisible = !_listVisible;

			if(_listVisible)
			{
				ShowCompareOptions(_searchValue);
			}else{
				HideDropDown();
			}
		}

		public void Clear()
		{
			_inputField.text = "";
			//_inputField.caretPosition = 0;
			_scrollRect.gameObject.SetActive(false);
			_dropDownpanel.gameObject.SetActive(false);
			//_arrowRect.DORotate(new Vector3(0, 0, 0), 0.3f);
			//_arrowRect.localScale = new Vector3(1, 1, 1);
		}

		public void ToggleListVisible()
		{
			_listVisible = !_listVisible;

			if (!_listVisible)
			{
				HideDropDown();
				return;
			}

			if (_listVisible)
			{
				ShowCompareOptions();
			}

		}

		public void HideDropDown()
		{
			_listVisible = false;
			_scrollRect.gameObject.SetActive(false);
			//_arrowRect.DORotate(new Vector3(0, 0, 0), 0.3f);
			_dropDownpanel.gameObject.SetActive(false);
			//_arrowRect.localScale = new Vector3(1, 1, 1);
		}

		public void SetCoutryList(List<CountryData> newOptions)
		{
			_countries = newOptions;
		}

		private void ShowCompareOptions(string value = "")
		{
			_listVisible = true;
			int max = 0;

			_dropDownpanel.gameObject.SetActive(true);
			_scrollRect.gameObject.SetActive(true);
			//_arrowRect.DORotate(new Vector3(0, 0, 180), 0.3f);
			//_arrowRect.localScale = new Vector3(1, -1, 1);
			_listItems.HideAll();
			var _optionsVisible = _countries.FindAll(x => x.title.ToLower().Contains(value.ToLower()));

			for (int i = 0; i < _optionsVisible.Count; i++)
			{
				var item = _listItems.GetItem();

				var textUi = item.GetComponentInChildren<TextMeshProUGUI>();
				var itm = _optionsVisible[i];
				string text = itm.title;
				textUi.text = text;

				item.onClick.RemoveAllListeners();
				item.onClick.AddListener(() =>
				{
					_inputField.text = text;
					_resultField.text = text;
					HideDropDown();
					onChangeValue?.Invoke(text);
					_selectedElement = itm;
				});
				max = Mathf.Min(_maxItems, _optionsVisible.Count);

			}

			RectTransform scrollRect = _scrollRect.GetComponent<RectTransform>();
			RectTransform itmRect = _buttonItemPrefab.GetComponent<RectTransform>();
			scrollRect.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, max * itmRect.rect.height);
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, _optionsVisible.Count * itmRect.rect.height);
			_dropDownpanel.sizeDelta = new Vector2(_dropDownpanel.sizeDelta.x, (max+1.5f) * itmRect.rect.height);

		}

	}
}
