using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace it.UI.Elements
{
	public class InputFieldValidate : MonoBehaviour
	{
		//[SerializeField] private 

		public enum State
		{
			None,
			Info,
			Error
		}

		public TMP_InputField InputField
		{
			get
			{
				if (_inputField == null)
					_inputField = GetComponent<TMP_InputField>();
				return _inputField;
			}
		}

		[SerializeField] private TextMeshProUGUI _infoLabel;
		[SerializeField] private Sprite _normalBack;
		[SerializeField] private Sprite _infoBack;
		[SerializeField] private Sprite _errorBack;

		[SerializeField] private Image _infoButton;
		[SerializeField] private Sprite _infoIcone;
		[SerializeField] private Sprite _errorIcone;

		[SerializeField] private Image _infoPanel;
		[SerializeField] private bool setAsParentSibling = true;
		[SerializeField] private Sprite _errorInfoBack;
		[SerializeField] private Sprite _infoInfoBack;

		[SerializeField] private State _defaultState = State.None;
		[FormerlySerializedAs("IsNewUsers")][SerializeField] private bool isOldUser;
		[SerializeField] private Image _bgImage;

		private TMP_InputField _inputField;
		private State _state;
		private bool _isVisibleInfo = false;
		private string _errorText;


		private void OnEnable()
		{
			//	SetState(_defaultState);
			if (_infoPanel != null && setAsParentSibling)
			{
				_infoPanel.transform.SetParent(transform.parent);
				_infoPanel.transform.SetAsLastSibling();
			}
		}

		private void Awake()
		{
			if (_bgImage == null)
				_bgImage = GetComponent<Image>();
			/*InputField.onSelect.RemoveAllListeners();
			InputField.onSelect.AddListener((string str) => { SetState(_defaultState); });*/
		}

		private void SetState(State newState)
		{
			_state = newState;

			switch (_state)
			{
				case State.Info:
					{
						if (!isOldUser)
						{

							/*_bgImage.sprite = _infoBack;
							_infoPanel.gameObject.SetActive(false);
							_infoButton.gameObject.SetActive(true);
							_infoButton.sprite = _infoIcone;
							_infoPanel.sprite = _infoInfoBack;
							_infoLabel.color = Color.black;*/

						}
						break;
					}
				case State.Error:
					{


						if (!isOldUser)
						{
							_bgImage.sprite = _errorBack;

							if (_infoButton != null)
							{
								_infoButton.gameObject.SetActive(true);
								_infoButton.sprite = _errorIcone;
							}

							if (_infoPanel != null)
							{
								_infoPanel.gameObject.SetActive(true);
								_infoPanel.sprite = _errorInfoBack;
							}

							if (_infoLabel != null)
								_infoLabel.color = Color.white;
						}

						break;
					}
				case State.None:
					{
						if (!isOldUser)
						{
							if (_infoButton != null)
							{
								_infoButton.gameObject.SetActive(false);
								_infoButton.sprite = _infoIcone;
							}


							_bgImage.sprite = _normalBack;


							if (_infoPanel != null)
							{
								_infoPanel.sprite = _infoInfoBack;
								_infoPanel.gameObject.SetActive(false);
							}

							_isVisibleInfo = false;
						}

						break;
					}
			}
		}

		public void SetInfo(State state, string errorText = "")
		{

			_errorText = errorText;
			if (_infoLabel != null)
			{
				_infoLabel.text = errorText;

			}

			SetState(state);
		}

		public void InfoButton()
		{
			_isVisibleInfo = !_isVisibleInfo;

			if (_isVisibleInfo)
			{
				_infoPanel.gameObject.SetActive(true);
				_bgImage.sprite = _state == State.Error ? _errorBack : _infoBack;
			}
			else
			{
				_infoPanel.gameObject.SetActive(false);
				_bgImage.sprite = _normalBack;
			}


		}
	}
}