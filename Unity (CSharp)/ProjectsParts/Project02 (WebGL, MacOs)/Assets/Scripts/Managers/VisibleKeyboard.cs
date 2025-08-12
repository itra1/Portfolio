using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
#if UNITY_WEBGL
using VirtualKeyboard;
#endif
using UnityEngine.UI;

public class VisibleKeyboard : Singleton<VisibleKeyboard>
{
	[SerializeField] private RectTransform _keyboard;
	[SerializeField] private TMP_InputField _inputAnswer;
	[SerializeField] private GameObject _closeButton;
	[SerializeField] private UnityEngine.UI.Selectable _submit;
	[SerializeField] private RectTransform _disableEye;
	[SerializeField] private RectTransform _enableEye;

	private TMP_InputField _targetInput;
	private bool _isVisible;
	private bool _eyeVisible;

	private void Start()
	{
#if UNITY_WEBGL
		_keyboard.gameObject.SetActive(false);
		_closeButton.gameObject.SetActive(false);
		_inputAnswer.caretWidth = 2;
		_submit.interactable = false;
		_inputAnswer.onValueChanged.AddListener((val)=> {
			_submit.interactable = val.Length > 0;
		});
		_keyboard.GetComponent<KeyboardFunc>().onKeyPressEvent = (string character) =>
		{
			int idx = _inputAnswer.caretPosition;

			if(_inputAnswer.characterLimit > 0 && _inputAnswer.text.Length >= _inputAnswer.characterLimit)
			{
				_inputAnswer.Select();
				return;
			}

			_inputAnswer.text = _inputAnswer.text.Insert(idx, character);
			_inputAnswer.caretPosition += character.Length;
			_inputAnswer.Select();
		};
		_keyboard.GetComponent<KeyboardFunc>().onBackKeyPressEvent = () =>
		{
			int idx = _inputAnswer.caretPosition;
			if (idx == 0) return;
			_inputAnswer.text = _inputAnswer.text.Remove(idx - 1, 1);
			if (idx < _inputAnswer.text.Length)
				_inputAnswer.caretPosition--;
			else
				_inputAnswer.caretPosition = _inputAnswer.text.Length;
			_inputAnswer.Select();

		};
		_keyboard.GetComponent<KeyboardFunc>().onEnterKeyPressEvent = () =>
		{
			Close();
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			_targetInput.text = _inputAnswer.text;
			_inputAnswer.text = "";
		};
		#endif
	}

	public void Close()
	{
		_keyboard.DOAnchorPos(new Vector2(0, -_keyboard.rect.height), 0.2f).OnComplete(() =>
		{
			_keyboard.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
		});
	}

	public void Visible(TMP_InputField targetInput)
	{
		_closeButton.gameObject.SetActive(true);
		_targetInput = targetInput;
		if (!_keyboard.gameObject.activeSelf)
		{
			_keyboard.gameObject.SetActive(true);
			_keyboard.anchoredPosition = new Vector2(0, -_keyboard.rect.height);
			_keyboard.DOAnchorPos(Vector2.zero, 0.2f);
			_inputAnswer.caretPosition = _inputAnswer.text.Length;
		}
		_inputAnswer.contentType = targetInput.contentType;
		_inputAnswer.lineType = targetInput.lineType;
		_inputAnswer.characterLimit = targetInput.characterLimit;
		_inputAnswer.text = _targetInput.text;
		_inputAnswer.Select();

		_disableEye.transform.parent.gameObject.SetActive(_inputAnswer.contentType == TMP_InputField.ContentType.Password);
		_eyeVisible = _inputAnswer.contentType == TMP_InputField.ContentType.Password;
		SetVisibleEye(false);
	}

	public void EyeToggle()
	{
		int idx = _inputAnswer.caretPosition;
		_disableEye.gameObject.SetActive(!_disableEye.gameObject.activeSelf);
		_enableEye.gameObject.SetActive(!_enableEye.gameObject.activeSelf);
		_inputAnswer.contentType = _enableEye.gameObject.activeSelf
															? TMP_InputField.ContentType.Name
															: TMP_InputField.ContentType.Password;
		
		_inputAnswer.caretPosition = idx;
		_inputAnswer.Select();
		string txt = _inputAnswer.text;
		_inputAnswer.text = "";
		_inputAnswer.text = txt;
		_inputAnswer.caretPosition = idx;
		_inputAnswer.selectionFocusPosition = idx;
	}

	private void SetVisibleEye(bool isVidible){
		_disableEye.gameObject.SetActive(!isVidible);
		_enableEye.gameObject.SetActive(isVidible);
	}

}