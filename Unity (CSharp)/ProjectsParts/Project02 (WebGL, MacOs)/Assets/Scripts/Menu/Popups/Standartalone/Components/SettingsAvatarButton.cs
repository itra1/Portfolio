using it.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsAvatarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public UnityEngine.Events.UnityAction<AvatarObject> OnClick;

	[SerializeField] private it.UI.Avatar _avatar;
	[SerializeField] private GameObject _selectBack;
	[SerializeField] private GameObject _hoverBack;

	private bool _isSelect;
	private AvatarObject _avatarObject;
	public AvatarObject AvatarObject { get => _avatarObject; set => _avatarObject = value; }

	private void OnEnable()
	{
		if (_hoverBack != null) _hoverBack.gameObject.SetActive(false);

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

#endif

	}

	public void SetAvatar(AvatarObject avatarObject)
	{
		_avatarObject = avatarObject;
		_avatar.SetDefaultAvatar();
		_avatar.SetAvatar(_avatarObject.url);
	}

	public bool IsSelect
	{
		get
		{
			return _isSelect;
		}
		set
		{
			_isSelect = value;
			if(_selectBack != null)	_selectBack.SetActive(_isSelect);
		}
	}

	public void ClickButton()
	{
		//_selectBack.SetActive(false);
		OnClick?.Invoke(_avatarObject);
		it.Logger.Log($"Im clicked {gameObject.name}");

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (_hoverBack != null)	_hoverBack.gameObject.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_hoverBack != null)	_hoverBack.gameObject.SetActive(false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		//_selectBack.SetActive(true);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		//_selectBack.SetActive(false);
	}
}
