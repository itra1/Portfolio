using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.UI.FirtMenu
{
  public class Button : MonoBehaviourBase, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
  {
	 public UnityEngine.Events.UnityAction<Button> onFocus;
	 public UnityEngine.Events.UnityAction onClick;

	 [SerializeField]
	 private Image _imageText = null;

	 private float _noActiveAlpha = .4f;

	 private bool isFocus = false;

	 private StateType _state = StateType.none;
	 private enum StateType
	 {
		none,
		focus,
		pointDown
	 }

	 public void SetSelect(Button btn)
	 {
		isFocus = btn.Equals(this);
		_imageText.color = isFocus
		  ? new Color(1, 1, 1, 1) 
		  : new Color(1, 1, 1, _noActiveAlpha);
	 }

	 public void OnPointerEnter(PointerEventData eventData)
	 {
		onFocus?.Invoke(this);
	 }

	 public void OnPointerDown(PointerEventData eventData)
	 {
		Debug.Log("Down");
	 }

	 public void OnPointerUp(PointerEventData eventData)
	 {
		Debug.Log("Up");
	 }

	 public void OnPointerClick(PointerEventData eventData)
	 {
		onClick?.Invoke();
	 }
  }
}