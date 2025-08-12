using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace it.UI.Elements
{

	/// <summary>
	/// Панель ввода с выпадающим списком
	/// </summary>
	public class InputDropDown : MonoBehaviour
	{
		[HideInInspector] public UnityEngine.Events.UnityEvent<string> onChangeValue = new UnityEngine.Events.UnityEvent<string>();
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private TextButtonUI _buttonItemPrefab;
		[SerializeField] private int _maxItems;
		[SerializeField] private RectTransform _arrowRect;

		private PoolList<TextButtonUI> _listItems;
		private List<string> _options = new List<string>();
		private bool _listVisible;

		public List<string> Options { get => _options; set => _options = value; }
		public int Value => Options.IndexOf(_inputField.text);

		public void OnEnable()
		{
			Clear();
			if (_listItems == null)
				_listItems = new PoolList<TextButtonUI>(_buttonItemPrefab.gameObject, _scrollRect.content);


			//_scrollRect = GetComponentInChildren<ScrollRect>();
			//_scrollRect.gameObject.SetActive(false);
			//_arrowRect.localScale = new Vector3(1, 1, 1);
			_scrollRect.vertical = true;
			_inputField.onValueChanged.RemoveAllListeners();
			_inputField.onValueChanged.AddListener((string value) =>
			{
				ShowCompareOptions(value);
			});
		}

		public void Clear()
		{
			_inputField.text = "";
			_inputField.caretPosition = 0;
			_scrollRect.gameObject.SetActive(false);
			_arrowRect.DORotate(new Vector3(0, 0, 0), 0.3f);
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
			_arrowRect.DORotate(new Vector3(0, 0, 0), 0.3f);
			//_arrowRect.localScale = new Vector3(1, 1, 1);
		}

		public void SetOptions(List<string> newOptions)
		{
			_options = newOptions;
		}

		private void ShowCompareOptions(string value = "")
		{
			_listVisible = true;
			int max = 0;

			_scrollRect.gameObject.SetActive(true);
			_arrowRect.DORotate(new Vector3(0, 0, 180), 0.3f);
			_arrowRect.localScale = new Vector3(1, -1, 1);
			_listItems.HideAll();
			List<string> _optionsVisible = _options.FindAll(x => x.ToLower().Contains(value.ToLower()));

			for (int i = 0; i < _optionsVisible.Count; i++)
			{
				var item = _listItems.GetItem();

				var textUi = item.GetComponentInChildren<TextMeshProUGUI>();
				string text = _optionsVisible[i];
				textUi.text = text;

				item.OnClick.RemoveAllListeners();
				item.OnClick.AddListener(() =>
				{
					_inputField.text = text;
					HideDropDown();
					onChangeValue?.Invoke(text);
				});
				max = Mathf.Min(_maxItems, _optionsVisible.Count);

			}


			RectTransform scrollRect = _scrollRect.GetComponent<RectTransform>();
			RectTransform itmRect = _buttonItemPrefab.GetComponent<RectTransform>();
			scrollRect.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, max * itmRect.rect.height);
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, _optionsVisible.Count * itmRect.rect.height);

		}

		private void Update()
		{

		}

	}



}