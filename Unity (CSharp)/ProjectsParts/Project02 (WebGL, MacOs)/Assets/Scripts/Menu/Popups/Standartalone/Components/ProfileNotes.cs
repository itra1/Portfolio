using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using Sett = it.Settings;
using System;

namespace it.Popups
{
	public class ProfileNotes : MonoBehaviour
	{
		[SerializeField] private ScrollRect _scroll;
		[SerializeField] private TMP_InputField _noteInput;
		[SerializeField] private TextMeshProUGUI _maxInputSymbolsLabel;
		[SerializeField] private Image[] _imageButtons;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;

		private UserNote _note;
		private int _maxInputSymbols = 1000;
		private int _activeIndex = 0;
		private RectTransform _textRect;

		public void Set(UserNote note)
		{
			_activeIndex = -1;
			_note = note;

			if (_note == null)
				_note = new UserNote();

			if (_note.message == null)
				_note.message = "";

			for (int i = 0; i < _imageButtons.Length; i++)
			{
				var button = _imageButtons[i].GetComponent<it.UI.Elements.ClearButtonUI>();
				button.OnClick.RemoveAllListeners();
				int index = i+1;
				button.OnClick.AddListener(() =>
				{
					ConfirmLabels(index);
				});
				//_imageButtons[i].material = Instantiate(_imageButtons[i].material);
				//_imageButtons[i].material.SetColor("_Fill", Sett.Settings.Sticks.Sticks[i].Color);
			}
			ConfirmLabels(_note.color);
			_applyButton.interactable = false;
			_noteInput.text = _note.message;
			_maxInputSymbolsLabel.text = $"{_note.message.Length} / {_maxInputSymbols}";
			_noteInput.onValueChanged.RemoveAllListeners();
			_noteInput.onValueChanged.AddListener((val) =>
			{
				if (val.Length > _maxInputSymbols)
					_noteInput.text = val.Substring(0, _maxInputSymbols);
				_maxInputSymbolsLabel.text = $"{val.Length} / {_maxInputSymbols}";
				_applyButton.interactable = true;
				//ConfirmNote();
			});
			//ConfirmNote();
		}

		private void ConfirmNote()
		{
			if (_textRect == null)
				_textRect = _noteInput.GetComponent<RectTransform>();
			_textRect.sizeDelta = new Vector2(_textRect.sizeDelta.x, _noteInput.preferredHeight);
			_scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, Mathf.Max(_scroll.viewport.rect.height, _textRect.rect.height));
		}

		private void ConfirmLabels(int index)
		{
			if (index == _activeIndex)
				index = 0;

			_activeIndex = index;
			_applyButton.interactable = true;

			for (int i = 0; i < _imageButtons.Length; i++)
			{
				int targetColorIndex = i + 1;
				Color light = Sett.AppSettings.Sticks.Sticks[targetColorIndex].Color;
				Color dark = light / 2;
				dark.a = 1;
				var check = _imageButtons[i].transform.Find("check").GetComponent<Image>();
				bool isSelect = _activeIndex == targetColorIndex;
				check.gameObject.SetActive(isSelect);
				_imageButtons[i].color = isSelect ? light : dark;

			}
		}

		public void LabelTouch()
		{
			_note.color = _activeIndex;
			_note.message = _noteInput.text;
			PlayerPrefs.SetString(StringConstants.NoteUpdate(_note.user_id), DateTime.Now.Millisecond.ToString());
			PlayerPrefs.Save();
			_note.Save();
			_applyButton.interactable = false;
		}

	}
}