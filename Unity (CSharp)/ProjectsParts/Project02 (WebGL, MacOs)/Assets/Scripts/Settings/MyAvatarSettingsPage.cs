using it.UI.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyAvatarSettingsPage : MonoBehaviour
{
	[SerializeField] private List<it.UI.Elements.GraphicButtonUI> _buttonsList;
	[SerializeField] private Sprite _activeSprite;
	[SerializeField] private Sprite _disactiveSprite;
	private int _selectCategory = 0;

	private void OnEnable()
	{
		ClickButton(0);
	}

	public void CustomButton()
	{
		it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
	}

	public void ClickButton(int index)
	{
		_selectCategory = index;
		it.Logger.Log($"click button{index}");
		ConfirmButtons();
	}

	public void ConfirmButtons()
	{

		for (int i = 0; i < _buttonsList.Count; i++)
		{
			bool isActive = i == _selectCategory;
			_buttonsList[i].GetComponent<Image>().sprite = isActive ? _activeSprite : _disactiveSprite;
			_buttonsList[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().color = isActive ? Color.black : Color.white;
		}
		SpawnAvatars(_selectCategory - 1);
	}
	[SerializeField]
	private SettingsAvatarButton _avatarPrefab;
	[SerializeField]
	private ScrollRect _scrollRect;
	[SerializeField]
	private GraphicButtonUI _applyButton;
	private PoolList<SettingsAvatarButton> _poolList;
	private List<SettingsAvatarButton> _itemsList;
	private ulong? _selectIndex;
	private void SpawnAvatars(int category)
	{
		_applyButton.interactable = false;
		if (_poolList == null) _poolList = new PoolList<SettingsAvatarButton>(_avatarPrefab.gameObject, _scrollRect.content);
		if (_itemsList == null) _itemsList = new List<SettingsAvatarButton>();
		_poolList.HideAll();
		if (UserController.ReferenceData == null) return;
		var categories = UserController.ReferenceData.avatar_categories;
		_itemsList.Clear();
		if (category >= 0 & category <= categories.Length)
		{
			var cat = categories[category];
			it.Logger.Log(cat.name);
			if (cat == categories[category])
				foreach (var ava in cat.avatars)
				{
					var pref = _poolList.GetItem();
					pref.SetAvatar(ava);
					 
					pref.IsSelect = false;
					pref.OnClick = AvaClick;

					_itemsList.Add(pref);
				}
		}
		//показать все категории
		if (category == -1)
		{
			foreach (var cat in categories)
				foreach (var ava in cat.avatars)
				{
					var pref = _poolList.GetItem();
					pref.SetAvatar(ava);
					pref.IsSelect = false;
					pref.OnClick = AvaClick;
					_itemsList.Add(pref);
				}
		}
		int rowCount = (int)Mathf.Ceil(_itemsList.Count / 5f);

		var gl = _scrollRect.content.GetComponent<GridLayoutGroup>();

		float h = (rowCount * gl.cellSize.y) + ((rowCount - 1) * gl.spacing.y);

		_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, h);
	}

	private void AvaClick(AvatarObject ava)
	{

		if (_selectIndex == ava.id)
		{
			_selectIndex = null;
			_applyButton.interactable = false;
		}
		else
		{
			_applyButton.interactable = true;
			_selectIndex = ava.id;
		}
		for (int i = 0; i < _itemsList.Count; i++)
		{
			_itemsList[i].IsSelect = _itemsList[i].AvatarObject.id == _selectIndex;
		}
	}

	public void ApplayButton()
	{
		var prof = UserController.User.user_profile.PostProfile;
		prof.avatar_id = _selectIndex;
		UserController.Instance.UpdateProfile(prof);
	}

	public void CancelButton()
	{

	}
}
