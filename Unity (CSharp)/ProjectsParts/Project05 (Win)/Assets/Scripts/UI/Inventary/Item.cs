using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.UI.Inventary
{
  public class Item : MonoBehaviourBase, IPointerClickHandler
  {
	 private it.Game.Items.Item _item;
	 public UnityEngine.Events.UnityAction<Item> onClic;

	 [SerializeField]
	 private Image _image;
	 [SerializeField]
	 private Image _border;

	 public void Clear()
	 {
		_image.sprite = null;
		_image.color = new Color(1, 1, 1, 0);
	 }

	 public void SetItem(it.Game.Items.Item item)
	 {
		_item = item;
		_image.sprite = _item.Icon;
		if (_item.Icon != null)
		{
		  var aspect = _image.GetComponent<AspectRatioFitter>();
		  aspect.aspectRatio = _item.Icon.rect.width / _item.Icon.rect.height;
		  _image.color = new Color(1, 1, 1, 1);
		}else
		  _image.color = new Color(1, 1, 1, 0);
	 }

	 public void OnPointerClick(PointerEventData eventData)
	 {

	 }
  }
}