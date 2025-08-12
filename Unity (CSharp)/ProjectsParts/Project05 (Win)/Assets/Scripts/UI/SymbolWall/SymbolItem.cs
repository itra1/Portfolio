using System.Collections;
using System.Collections.Generic;

using com.ootii.Items;

using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Symbols
{
  public class SymbolItem : MonoBehaviourBase
  {
	 private Image _item;

	 public void Clear()
	 {
		while (transform.childCount > 0)
		  DestroyImmediate(transform.GetChild(0).gameObject);
	 }

	 public void SetItem(it.Game.Items.Symbols.Symbol symbol)
	 {

		GameObject inst = Instantiate(symbol.UiIten.gameObject,transform);

		inst.transform.localPosition = Vector3.zero;
		inst.transform.localRotation = Quaternion.identity;
		inst.transform.localScale = Vector3.one * 0.1751881f;
		inst.GetComponent<SymbolAnimItem>().Full.color = Color.white;
		inst.gameObject.SetActive(true);

		//if (_item == null)
		//  _item = GetComponentInChildren<Image>();

		//_item.enabled = true;
		//_item.sprite = symbol.Icon;
		//_item.GetComponent<AspectRatioFitter>().aspectRatio = symbol.Icon.rect.width / symbol.Icon.rect.height;
	 }

  }
}
