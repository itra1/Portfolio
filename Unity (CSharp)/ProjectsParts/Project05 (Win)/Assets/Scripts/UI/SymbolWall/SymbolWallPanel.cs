using System.Collections;
using System.Collections.Generic;

using it.Game;
using it.Game.Items.Symbols;
using it.Game.Managers;

using JetBrains.Annotations;

using UnityEngine;

namespace it.UI.Symbols
{
  public class SymbolWallPanel : UIDialog
  {
	 public static bool IsActive = false;
	 public static bool IsAnimateVisible { get; set; }
	 public bool CloaseReady { get => IsActive && !IsAnimateVisible; }
	 public Symbol New { get => _new; set => _new = value; }

	 // 0.1751881

	 [SerializeField]
	 private List<SymbolItem> _itemList = new List<SymbolItem>();

	 private Symbol _new;
	 private SymbolItem _si;

	 protected override void OnEnable()
	 {
		IsActive = true;
		_new = null;
		_si = null;
		base.OnEnable();
		GameManager.Instance.AddEscape(this);
		FillItems();

		StartCoroutine(PlayNew());
	 }

	 IEnumerator PlayNew()
	 {

		if (_new != null)
		{
		  yield return new WaitForSeconds(1f);
		  ShowNewItem();
		}
	 }

	 protected override void OnDisable()
	 {
		base.OnDisable();
		_isEscape = false;
		GameManager.Instance.RemoveEscape(this);
		IsActive = false;
		//IsAnimateVisible = false;
	 }

	 public static void ShowPanel()
	 {
		if (IsAnimateVisible) return;

		var inventaryPanel = UiManager.GetPanel<it.UI.Inventary.InventaryPanel>();
		var symbolWallPanel = UiManager.GetPanel<it.UI.Symbols.SymbolWallPanel>();

		if (inventaryPanel.CloaseReady)
		{
		  inventaryPanel.NextPageButton();
		  return;
		}
		if (Inventary.InventaryPanel.IsActive)
		  return;

		IsAnimateVisible = true;

		it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			  new Color32(0, 0, 0, 255), 0.5f, null, () =>
			  {
				 VisibleDialog(!IsActive);
			  }, () =>
			  {
				 if(symbolWallPanel.New == null)
				  IsAnimateVisible = false;
			  });

	 }

	 public static void VisibleDialog(bool setVisible)
	 {
		IsActive = setVisible;
		var panel = it.Game.Managers.UiManager.GetPanel<it.UI.Symbols.SymbolWallPanel>();
		panel.gameObject.SetActive(IsActive);
	 }

	 public void ShowNewItem()
	 {
		GetComponentInChildren<SymbolNew>().Play(_new, _si.GetComponent<RectTransform>(),()=> {

		  _new = null;
		  IsAnimateVisible = false;
		});
	 }

	 private void FillItems()
	 {
		foreach (var elem in _itemList)
		  elem.Clear();

		List<GameObject> symbols = it.Game.Managers.GameManager.Instance.SymbolsManager.GetAllPrefabs();

		for (int i = 0; i < symbols.Count; i++)
		{
		  if (it.Game.Managers.GameManager.Instance.SymbolsManager.NewUUID != null &&
			 it.Game.Managers.GameManager.Instance.SymbolsManager.NewUUID == symbols[i].GetComponent<Symbol>().Uuid)
		  {
			 _si = _itemList[i];
			 _new = symbols[i].GetComponent<Symbol>();
			 it.Game.Managers.GameManager.Instance.SymbolsManager.NewUUID = null;
			 return;

		  }

		  if (i < symbols.Count)
			 _itemList[i].SetItem(symbols[i].GetComponent<Symbol>());
		  else
			 _itemList[i].Clear();
		}

	 }

	 public void NextPageButton()
	 {
		if (IsAnimateVisible) return;
		IsAnimateVisible = true;

		it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
		  new Color32(0, 0, 0, 255), .5f, null, () =>
		  {
			 gameObject.SetActive(false);
		  }, () =>
		  {
			 if (_new == null)
				IsAnimateVisible = false;
			 var panel = it.Game.Managers.UiManager.GetPanel<it.UI.Inventary.InventaryPanel>();
			 panel.gameObject.SetActive(true);
		  });

	 }

	 public void PrevPageButton()
	 {
		if (IsAnimateVisible) return;
		IsAnimateVisible = true;

		it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
		  new Color32(0, 0, 0, 255), .5f, null, () =>
		  {
			 gameObject.SetActive(false);
		  }, () =>
		  {
			 if (_new == null)
				IsAnimateVisible = false;
			 var panel = it.Game.Managers.UiManager.GetPanel<it.UI.Inventary.InventaryPanel>();
			 panel.gameObject.SetActive(true);
		  });

	 }

	 public override void Escape()
	 {
		if (_isEscape)
		  return;
		_isEscape = true;
		IsAnimateVisible = true;
		it.Game.Managers.UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			  new Color32(0, 0, 0, 255), 1, null, () =>
			  {
				 gameObject.SetActive(false);

			  }, () =>
			  {
				 IsAnimateVisible = false;
			  });
	 }
  }
}