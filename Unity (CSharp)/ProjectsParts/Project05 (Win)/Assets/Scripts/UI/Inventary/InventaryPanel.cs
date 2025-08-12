using System.Collections;
using System.Collections.Generic;

using it.Game;
using it.Game.Managers;

using UnityEngine;

namespace it.UI.Inventary
{
  public class InventaryPanel : UIDialog
  {
	 public static bool IsActive { get; set; }
	 public static bool IsAnimateVisible { get; set; }

	 public bool CloaseReady { get => IsActive && !IsAnimateVisible; }

	 [SerializeField]
	 private List<Item> _inventaryList = new List<Item>();

	 [SerializeField]
	 private List<Item> _carmicList = new List<Item>();

	 protected override void OnEnable()
	 {
		IsActive = true;
		base.OnEnable();
		GameManager.Instance.AddEscape(this);
		FillItems();
		FillCarmic();
		IsActive = true;
	 }

	 protected override void OnDisable()
	 {
		base.OnDisable();
		GameManager.Instance.RemoveEscape(this);
		IsActive = false;
		IsAnimateVisible = false;
		IsActive = false;
	 }

	 public void OnAnimateStart()
	 {
		IsAnimateVisible = IsActive;
	 }

	 public void OnAnimateStop()
	 {
		IsAnimateVisible = !IsActive;
	 }

	 public static void ShowPanel()
	 {
		if (IsAnimateVisible) return;

		var symbolsPanel = UiManager.GetPanel<it.UI.Symbols.SymbolWallPanel>();

		if (symbolsPanel.CloaseReady)
		{
		  UiManager.GetPanel<it.UI.Symbols.SymbolWallPanel>().NextPageButton();
		  return;
		}
		if (Symbols.SymbolWallPanel.IsActive)
		  return;


		var panel = it.Game.Managers.UiManager.GetPanel<it.UI.Inventary.InventaryPanel>();
		var gameUI = it.Game.Managers.UiManager.GetPanel<it.UI.Game.GameUI>();

		panel.onEnamble = () =>
		{
		  gameUI.gameObject.SetActive(false);

		  it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = false;
		};
		panel.onDisable = () =>
		{
		  gameUI.gameObject.SetActive(true);
		  it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = true;
		};
		if (!IsActive)
		{
		  panel.Show();
		}
		else
		  panel.Hide();
	 }

	 private void FillItems()
	 {
		foreach (var elem in _inventaryList)
		  elem.Clear();

		List<GameObject> objects = it.Game.Managers.GameManager.Instance.Inventary.GetAllPrefabs();

		int index = 0;

		foreach (var item in objects)
		{
		  if (_inventaryList.Count > index)
		  {
			 var comp = item.GetComponent<it.Game.Items.Inventary.InventaryItem>();

			 if (comp.IsSystem)
				continue;

			 if (!comp.IsCarmic)
			 {
				_inventaryList[index].SetItem(comp);
			 }
		  }
		  index++;
		}


	 }

	 private void FillCarmic()
	 {

		foreach (var elem in _carmicList)
		  elem.Clear();

		List<GameObject> objects = it.Game.Managers.GameManager.Instance.Inventary.GetAllPrefabs();

		int index = 0;

		foreach (var item in objects)
		{
		  if (_carmicList.Count > index)
		  {
			 var comp = item.GetComponent<it.Game.Items.Inventary.InventaryItem>();
			 if (comp.IsCarmic)
			 {
				_carmicList[index].SetItem(comp);
			 }
		  }
		}
	 }

	 public override void Hide(float timeHide = 1)
	 {
		base.Hide(timeHide);
	 }

	 public void NextPageButton()
	 {
		if (IsAnimateVisible) return;
		IsAnimateVisible = true;

		Hide();

		it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			  new Color32(0, 0, 0, 255), 0.5f, null, () =>
			  {
				 it.UI.Symbols.SymbolWallPanel.VisibleDialog(true);
			  }, () =>
			  {
				 IsAnimateVisible = false;
			  });
	 }

	 public void PrevPageButton()
	 {
		if (IsAnimateVisible) return;
		IsAnimateVisible = true;

		Hide();

		it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			  new Color32(0, 0, 0, 255), 0.5f, null, () =>
			  {
				 it.UI.Symbols.SymbolWallPanel.VisibleDialog(true);

			  }, () =>
			  {
				 IsAnimateVisible = false;
			  });


	 }

	 public override void Escape()
	 {
		if (_isEscapeProcess)
		  return;
		_isEscapeProcess = true;
		GameManager.Instance.InventaryPanel();
	 }
  }
}